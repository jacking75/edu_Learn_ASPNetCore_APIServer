using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;


namespace ContentDLL;

class DBWork
{
    MySqlCompiler _compiler = new();

    

    public async Task DeleteAccount(string connString, string userID)
    {
        try
        {
            using var conn = await OpenConnectionAsync(connString);
            QueryFactory queryFactory = new(conn, _compiler);

            string user_id = userID;
            await queryFactory.Query("account").Where(new { user_id }).DeleteAsync();            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }



    public async Task<DAO.Account> GetAccountByUsername(string connString, string username)
    {
        try
        {
            using var conn = await OpenConnectionAsync(connString);
            QueryFactory queryFactory = new(conn, _compiler);

            var parameters = new { username };
            var account = await queryFactory.Query("account")
                .Where(parameters)
                .FirstOrDefaultAsync<DAO.Account>();

            return account;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }


    async Task<MySqlConnection> OpenConnectionAsync(string connString)
    {
        try
        {
            if (string.IsNullOrEmpty(connString))
            {
                Console.WriteLine($"Connection string is null or empty");
                return null;
            }

            var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            return conn;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}
