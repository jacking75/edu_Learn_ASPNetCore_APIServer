using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using SqlKata.Execution;

namespace APIServer.MasterData;

public class Manager : IManager
{
    private readonly Dictionary<long, ItemData> _itemDatas = new();

    public bool Load(string connectionString)
    {
        using MySqlConnection connection = new(connectionString);
        QueryFactory queryFactory = CraeteQueryFactory(connection);

        _ = LoadItem(queryFactory);

        return true;
    }

    private QueryFactory CraeteQueryFactory(MySqlConnection connection)
    {
        SqlKata.Compilers.MySqlCompiler compiler = new();
        return new QueryFactory(connection, compiler);
    }

    private async Task<bool> LoadItem(QueryFactory queryFactory)
    {
        IEnumerable<ItemData> loadedDatas = await queryFactory.Query($"MasterDb.Item").GetAsync<ItemData>();
        if (loadedDatas is null)
        {
            return false;
        }

        foreach (ItemData loadedData in loadedDatas)
        {
            _itemDatas.Add(loadedData.ItemCode, loadedData);
        }

        return true;
    }


    public void Dispose()
    {
    }

    Task<bool> IManager.Load(string connectionString)
    {
        throw new System.NotImplementedException();
    }
}
