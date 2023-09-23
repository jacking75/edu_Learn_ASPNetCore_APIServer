using System.Security.Cryptography;
using System.Text;
using CloudStructures;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;

public class DBManager
{
    static string GameDBConnectString;
    static string RedisAddress;

    public static RedisConnection RedisConn { get; set; }
    
    
    public static void Init(IConfiguration configuration)
    {
        GameDBConnectString = configuration.GetSection("DBConnection")["MySqlGame"];
        RedisAddress = configuration.GetSection("DBConnection")["Redis"];
        
        var config = new RedisConfig("basic", RedisAddress);
        RedisConn = new RedisConnection(config);
    }

    
    public static async Task<QueryFactory> GetGameDBQuery()
    {
        var connection = new MySqlConnection(GameDBConnectString);
        await connection.OpenAsync();
        
        var compiler = new SqlKata.Compilers.MySqlCompiler();
        var queryFactory = new SqlKata.Execution.QueryFactory(connection, compiler);
        
        return queryFactory;
    }
    

    
}
