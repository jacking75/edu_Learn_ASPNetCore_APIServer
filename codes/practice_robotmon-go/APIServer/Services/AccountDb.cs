using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ApiServer.Model;
using ApiServer.Options;
using CloudStructures;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using ServerCommon;
using ZLogger;

namespace ApiServer.Services
{
    public class AccountDb : IAccountDb
    {
        private readonly IOptions<DbConfig> _dbConfig;
        private IDbConnection _dbConn;
        private IDbTransaction _dBTransaction;
        private readonly ILogger<AccountDb> _logger;
        private bool _isDisposed = false;

        public AccountDb(ILogger<AccountDb> logger, IOptions<DbConfig> dbConfig)
        {
            _dbConfig = dbConfig;
            _logger = logger;
            Open();
        }

        protected virtual void Dispose(bool _disposing)
        {
            if (!_isDisposed)
            {
                if (_disposing)
                {
                    Close();
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Open()
        {
            _dbConn = new MySqlConnection(_dbConfig.Value.AccountConnStr);

            _dbConn.Open();
        }
        
        public void Close()
        {
            _dbConn.Close();
        }
        
        // 트랜잭션 시작.
        public void StartTransaction()
        {
            if (_dbConn == null)
            {
                throw new Exception("DB is not opened");
            }
            if (_dBTransaction != null)
            {
                throw new Exception("DB transaction is not finished");
            }

            _dBTransaction = _dbConn.BeginTransaction(IsolationLevel.RepeatableRead);

            if (_dBTransaction == null)
            {
                throw new Exception("DB transaction error");
            }
        }

        // 트랜잭션 롤백.
        public void Rollback()
        {
            if (_dbConn == null)
            {
                throw new Exception("DB is not opened");
            }
            if (_dBTransaction == null)
            {
                throw new Exception("DB transaction is not started");
            }
            
            _dBTransaction.Rollback();
            _dBTransaction = null;
        }

        // 트랜잭션 커밋.
        public void Commit()
        {
            if (_dbConn == null)
            {
                throw new Exception("DB is not opened");
            }
            if (_dBTransaction == null)
            {
                throw new Exception("DB transaction is not started");
            }

            _dBTransaction.Commit();
            _dBTransaction = null;
        }
        
        public async Task<Tuple<ErrorCode, Int64>> CreateAccountDataAsync(string id, string pw, string salt)
        {
            try
            {
                // StartTransaction과 Commit을 넣는 이유
                // 멀티스레드 환경에서 insert를 한뒤 select last_insert_id를 진행할때, 다른 스레드에서 insert를 진행한다면 엉뚱한 인덱스를 가져올 수 있습니다. 
                StartTransaction();
                var insertQuery = $"insert users(ID, PW, Salt) Values(@userId, @userPw, @userSalt); SELECT LAST_INSERT_ID();";
                var lastInsertId = await _dbConn.QueryFirstAsync<Int32>(insertQuery, new
                {
                    userId = id,
                    userPw = pw,
                    userSalt = salt
                });
                Commit();
                
                return new Tuple<ErrorCode, Int64>(ErrorCode.None, lastInsertId);
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(CreateAccountDataAsync)} Exception : {e}");
                return new Tuple<ErrorCode, Int64>(ErrorCode.CreateAccountFailDuplicate, 0);
            }
        }

        public async Task<ErrorCode> RollbackCreateAccountDataAsync(Int64 createIdx)
        {
            try
            {
                var deleteQuery = $"delete from users where UID = {createIdx}";
                var count = await _dbConn.ExecuteAsync(deleteQuery);
                
                if (count == 0)
                {
                    _logger.ZLogError($"{nameof(RollbackCreateAccountDataAsync)} Error : {ErrorCode.RollbackCreateAccountFailDeleteQuery}");
                    return ErrorCode.RollbackCreateAccountFailDeleteQuery;
                }
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(RollbackCreateAccountDataAsync)} Exception : {e}");
                return ErrorCode.RollbackSendCreateAccountFailException;
            }
        }
        
        public async Task<ErrorCode> TryPasswordAsync(string id, string pw)
        {
            try
            {
                var selectQuery = $"select PW, Salt from users where ID = @userId";
                var loginData = await _dbConn.QuerySingleOrDefaultAsync<TableLoginData>(selectQuery, new
                {
                    userId = id
                });
                
                if (loginData is null)
                {
                    return ErrorCode.LoginFailNoUserExist;
                }

                if (string.IsNullOrWhiteSpace(loginData.PW))
                {
                    return ErrorCode.LoginFailNoUserExist;
                }
                
                // password 일치 여부 확인
                var hashingPassword = Security.MakeHashingPassWord(loginData.Salt, pw);
                if (loginData.PW != hashingPassword)
                {
                    return ErrorCode.LoginFailWrongPassword;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(TryPasswordAsync)} Exception : {e}");
                return ErrorCode.LoginFailException;
            }

            return ErrorCode.None;
        }
    }
}