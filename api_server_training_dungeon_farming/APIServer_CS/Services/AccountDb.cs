using System;
using System.Data;
using System.Threading.Tasks;

using APIServer.ModelDB;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySqlConnector;

using SqlKata.Execution;

namespace APIServer.Services;

public class AccountDb : IAccountDb
{
    private readonly IOptions<DbConfig> _dbConfig;

    private readonly ILogger<AccountDb> _logger;

    private readonly SqlKata.Compilers.MySqlCompiler _compiler;

    private readonly QueryFactory _queryFactory;

    private IDbConnection _dbConn;


    public AccountDb(ILogger<AccountDb> logger, IOptions<DbConfig> dbConfig)
    {
        _dbConfig = dbConfig;
        _logger = logger;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, _compiler);
    }


    public void Dispose()
    {
        Close();
    }


    public async Task<bool> CreateAccount(string email, string password, Int64 userId)
    {
        try
        {
            var saltValue = Security.SaltString();
            var hashedPassword = Security.MakeHashingPassWord(saltValue, password);
            var insertedId = await _queryFactory
                .Query("AccountDB.account")
                .InsertGetIdAsync<Int64>(new
                {
                    user_id = userId,
                    email = email,
                    password = hashedPassword,
                    salt_value = saltValue
                });

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    public async Task<(ErrorCode, Account)> VerifyAccount(string email, string password)
    {
        try
        {
            var loadedData = await _queryFactory
                .Query("AccountDB.account")
                .Where("email", email)
                .FirstOrDefaultAsync<Account>();

            if (loadedData is null || loadedData.account_id == 0)
            {
                return (ErrorCode.NotExistAccount, null);
            }

            var hashValue = Security.MakeHashingPassWord(loadedData.salt_value, password);
            if (loadedData.password != hashValue)
            {
                return (ErrorCode.InvalidPassword, null);
            }

            return (ErrorCode.None, loadedData);
        }
        catch (Exception ex)
        {
            return (ErrorCode.AccountDbException, null);
        }
    }


    public async Task<ErrorCode> DeleteAccount(Int64 accountId, string email)
    {
        try
        {
            _ = await _queryFactory
                .Query("AccountDB.account")
                .Where("account_id", accountId)
                .Where("email", email)
                .DeleteAsync();

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            return ErrorCode.AccountDbException;
        }
    }


    private void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.AccountDb);
        _dbConn.Open();
    }


    private void Close()
    {
        _dbConn.Close();
    }
}

