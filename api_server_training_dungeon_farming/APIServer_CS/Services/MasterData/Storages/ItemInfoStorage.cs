using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;

using SqlKata.Execution;

namespace APIServer.Services.MasterData.Storages
{
    public class ItemInfoStorage
    {
        public Dictionary<Int32, ItemInfo> Datas { get; private set; } = null;

        private readonly string _tableName = "item_info";

        public async Task<bool> Load(QueryFactory queryFactory)
        {
            var loadedDatas = await MasterDataLoader.LoadAsList<ItemInfo>(queryFactory, _tableName);

            if (loadedDatas is null)
            {
                return false;
            }

            Store(loadedDatas);

            return true;
        }

        private void Store(List<ItemInfo> datas)
        {
            Datas = new Dictionary<Int32, ItemInfo>(datas.Count);
            foreach (var data in datas)
            {
                Datas.TryAdd(data.item_code, data);
            }
        }



    }



}
