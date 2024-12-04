using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using SqlKata.Execution;

namespace APIServer.Services.MasterData
{
    public class MasterDataLoader
    {
        private static readonly ILogger<MasterDataLoader> _logger = LogManager.GetLogger<MasterDataLoader>();

        public static async Task<List<T>> LoadAsList<T>(QueryFactory queryFactory, string tableName)
        {
            var loadedDatas = await queryFactory.Query($"MasterDb.{tableName}").GetAsync<T>();
            if (loadedDatas is null)
            {
                return null;
            }

            return loadedDatas.ToList();
        }



    }

}

