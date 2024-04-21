using System.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;

namespace basic2_04.Repository;

public class GameDB : IGameDB
{
    private readonly IOptions<DbConfig> _dbConfig;
    private IDbConnection? _dbConn;
    private readonly SqlKata.Compilers.MySqlCompiler _compiler;
    private readonly QueryFactory _queryFactory;


    public GameDB(IOptions<DbConfig> dbConfig)
    {
        _dbConfig = dbConfig;
        
        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
    }

    public void Dispose()
    {
        Close();
    }

    public async Task<Tuple<ErrorCode, long>> AuthCheck(string userID, string pw)
    {
        try
        {
            var userInfo = await _queryFactory.Query("users")
                                    .Where("id", userID)
                                    .FirstOrDefaultAsync<GameUser>();

            if (userInfo.uid == 0)
            {
                return new Tuple<ErrorCode, long>(ErrorCode.LoginFailUserNotExist, 0);
            }

            if (userInfo.pw != pw)
            {
                return new Tuple<ErrorCode, long>(ErrorCode.LoginFailPwNotMatch, 0);
            }

            return new Tuple<ErrorCode, long>(ErrorCode.None, userInfo.uid);
        }
        catch /*(Exception e)*/
        {
            return new Tuple<ErrorCode, long>(ErrorCode.LoginFailException, 0);
        }
    }

    private void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.GameDB);

        _dbConn.Open();
    }

    private void Close()
    {
        _dbConn?.Close();
    }
}


public class DbConfig
{
    public string GameDB { get; set; }
    public string Redis { get; set; }
}


public class GameUser
{
    public long uid { get; set; }
    public string id { get; set; }
    public string pw { get; set; }
}
