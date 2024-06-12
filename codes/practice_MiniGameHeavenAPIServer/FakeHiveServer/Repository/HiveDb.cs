using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using APIServer.Services;
using Microsoft.Extensions.Logging;
using ZLogger;
using APIServer.Model.DAO;

namespace APIServer.Repository
{
    public class HiveDb : IHiveDb
    {
        readonly IOptions<DbConfig> _dbConfig;
        readonly ILogger<HiveDb> _logger;
        IDbConnection _dbConn;
        readonly SqlKata.Compilers.MySqlCompiler _compiler;
        readonly QueryFactory _queryFactory;

        public HiveDb(ILogger<HiveDb> logger, IOptions<DbConfig> dbConfig)
        {
            _logger = logger;
            _dbConfig = dbConfig;

            Open();

            _compiler = new SqlKata.Compilers.MySqlCompiler();
            _queryFactory = new QueryFactory(_dbConn, _compiler);
        }

        public void Dispose()
        {
            Close();
        }

        public async Task<ErrorCode> CreateAccountAsync(string email, string pw)
        {
            try
            {
                var saltValue = Security.SaltString();
                var hashingPassword = Security.MakeHashingPassWord(saltValue, pw);

                var count = await _queryFactory.Query("account_info").InsertAsync(new HdbAccountInfo
                {
                    player_id = 0,
                    email = email,
                    salt_value = saltValue,
                    pw = hashingPassword,
                    create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                });

                _logger.ZLogDebug(
                $"[CreateAccount] email: {email}, salt_value : {saltValue}, hashed_pw:{hashingPassword}");

                return count != 1 ? ErrorCode.CreateAccountFailInsert : ErrorCode.None;
            }
            catch (Exception ex)
            {
                _logger.ZLogError(
                $"[AccoutDb.CreateAccount] ErrorCode: {ErrorCode.CreateAccountFailException}");
                return ErrorCode.CreateAccountFailException;
            }
        }

        public async Task<(ErrorCode, Int64)> VerifyUser(string email, string pw)
        {
            try
            {
                Model.DAO.HdbAccountInfo userInfo = await _queryFactory.Query("account_info")
                                        .Where("Email", email)
                                        .FirstOrDefaultAsync<Model.DAO.HdbAccountInfo>();
                
                if (userInfo is null)
                {
                    return (ErrorCode.LoginFailUserNotExist, 0);
                }              

                var hashingPassword = Security.MakeHashingPassWord(userInfo.salt_value, pw);
                if (userInfo.pw != hashingPassword)
                {
                    return (ErrorCode.LoginFailPwNotMatch, 0);
                }

                return (ErrorCode.None, userInfo.player_id);
            }
            catch (Exception e)
            {
                return (ErrorCode.LoginFailException, 0);
            }
        }

        void Open()
        {
            _dbConn = new MySqlConnection(_dbConfig.Value.HiveDb);
            _dbConn.Open();
        }
        void Close()
        {
            _dbConn.Close();
        }
    }
}


public class DbConfig
{
    public string HiveDb { get; set; }
}
