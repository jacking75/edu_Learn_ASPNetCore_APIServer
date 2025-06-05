using HiveServer.Repository.Interface;
using HiveServer.Models.DAO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;
using ZLogger;
using System.Transactions;

namespace HiveServer.Repository;

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
   
    void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.HiveDb);
        _dbConn.Open();
    }

    void Close() 
    {
        _dbConn.Close();
    }

    public async Task<int> DuplicateNickname(string nickname)
    {
        return await _queryFactory.Query("account")
            .Where("nickname", nickname).CountAsync<int>();
    }

    public async Task<ErrorCode> CreateAccount(string emailId, string password, string nickname)
    {
        try
        {
            var saltValue = Security.GenerateSaltString();
            var hashingPassword = Security.MakeHashingPassWord(saltValue, password);

            var count = await _queryFactory.Query("account")
                .InsertAsync(new HiveDBAccount
            {
                email_id = emailId,
                pw = hashingPassword,
                nickname = nickname,
                salt_value = saltValue,
                create_dt = DateTime.Now
            });

            _logger.ZLogDebug(
            $"[CreateAccount] email: {emailId}, salt_value : {saltValue}, hashed_pw:{hashingPassword}");

            return count != 1 ? ErrorCode.CreateAccountFail : ErrorCode.None;
        }
        catch(Exception ex) 
        {
            _logger.ZLogError(
               $"[HiveDb.CreateAccount] ErrorCode: {ErrorCode.CreateAccountFail}, {ex}");
            return ErrorCode.CreateAccountFail;
        }
    }

    public async Task<(ErrorCode, Int64)> VerifyUser(string emailId, string password) 
    {
        try
        {
            var userInfo = await _queryFactory.Query("account")
                .Where("email_id", emailId)
                .FirstOrDefaultAsync<HiveDBAccount>();

            if (userInfo == null)
            {
                return (ErrorCode.LoginFail, 0);
            }

            var hashingPassword = Security.MakeHashingPassWord(userInfo.salt_value, password);
            if(hashingPassword != userInfo.pw)
            {
                return (ErrorCode.LoginFail, 0);
            }

            return (ErrorCode.None, userInfo.account_uid);
        }
        catch (Exception ex)
        {
            _logger.ZLogError(
                $"[HiveDb.VerifyUser] ErrorCode: {ErrorCode.LoginFail}, {ex}");
            return (ErrorCode.LoginFail, 0);
        }
    }
}

public class DbConfig 
{
    public string HiveDb { get; set; }
    public string Redis { get; set; }
}