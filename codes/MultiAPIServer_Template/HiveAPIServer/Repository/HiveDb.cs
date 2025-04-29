using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using MatchAPIServer.Services;
using Microsoft.Extensions.Logging;
using ZLogger;
using MatchAPIServer.Model.DAO;


namespace MatchAPIServer.Repository;

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

    public async Task<ErrorCode> CreateAccount(string userID, string pw)
    {
        try
        {
            var saltValue = Security.SaltString();
            var hashingPassword = Security.MakeHashingPassWord(saltValue, pw);

            var count = await _queryFactory.Query("account").InsertAsync(new HiveDBAccount
            {
                user_id = userID,
                salt_value = saltValue,
                pw = hashingPassword,
                create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                recent_login_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            });

            _logger.ZLogDebug(
            $"[CreateAccount] email: {userID}, salt_value : {saltValue}, hashed_pw:{hashingPassword}");

            return count != 1 ? ErrorCode.CreateAccountFailInsert : ErrorCode.None;
        }
        catch (Exception ex)
        {
            _logger.ZLogError(
            $"[HiveDb.CreateAccount] ErrorCode: {ErrorCode.CreateAccountFailException}, {ex}");
            return ErrorCode.CreateAccountFailException;
        }
    }

    public async Task<(ErrorCode, Int64)> VerifyUser(string userID, string pw)
    {
        try
        {
            Model.DAO.HiveDBAccount userInfo = await _queryFactory.Query("account")
                                    .Where("user_id", userID)
                                    .FirstOrDefaultAsync<Model.DAO.HiveDBAccount>();
                
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
        catch (Exception ex)
        {
            _logger.ZLogError(
            $"[HiveDb.VerifyUser] ErrorCode: {ErrorCode.LoginFailException}, {ex}");
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



public class DbConfig
{
    public string HiveDb { get; set; }
}
