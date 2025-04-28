using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using SqlKata.Compilers;
using SqlKata.Execution;
using MySqlConnector;
using ServerShared.Repository.Interfaces;

namespace ServerShared.Repository;

public abstract class GameDb<T> : BaseLogger<T>, IGameDb<T>
{
    IDbConnection _dbConn;
    protected readonly MySqlCompiler _compiler;
    protected readonly QueryFactory _queryFactory;

    public GameDb(ILogger<T> logger, IOptions<ServerConfig> dbConfig) : base(logger)
    {
        Open(dbConfig);

        _compiler = new MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
    }

    public virtual Task<ErrorCode> Set(T data)
    {
        return Task.FromResult(ErrorCode.None);
    }

    public virtual Task<ErrorCode> Update(Int64 uid, object value)
    {
        return Task.FromResult(ErrorCode.None);
    }

    public virtual Task<ErrorCode> Delete(Int64 uid)
    {
        return Task.FromResult(ErrorCode.None);
    }

    public virtual Task<(ErrorCode, T?)> Get(Int64 uid)
    {
        return Task.FromResult<(ErrorCode, T?)>((ErrorCode.None, default));
    }

    public virtual Task<(ErrorCode, IEnumerable<T>?)> GetAll(Int64 uid)
    {
        return Task.FromResult<(ErrorCode, IEnumerable<T>?)>((ErrorCode.None, null));
    }

    private void Open(IOptions<ServerConfig> config)
    {
        _dbConn = new MySqlConnection(config.Value.GameDb);
        _dbConn.Open();
    }

    void Close()
    {
        _dbConn.Close();
    }

    public void Dispose()
    {
        Close();
    }
}
