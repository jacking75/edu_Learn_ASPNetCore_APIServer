using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;

using SqlKata.Execution;

namespace APIServer.Services.MasterData.Storages
{
    public class ItemTypeStorage
    {
        public Dictionary<Int32, ItemType> Datas { get; private set; } = null;

        private readonly string _tableName = "item_type";

        public async Task<bool> Load(QueryFactory queryFactory)
        {
            var loadedDatas = await MasterDataLoader.LoadAsList<ItemType>(queryFactory, _tableName);

            if (loadedDatas is null)
            {
                return false;
            }

            Store(loadedDatas);

            return true;
        }

        private void Store(List<ItemType> datas)
        {
            Datas = new Dictionary<Int32, ItemType>(datas.Count);
            foreach (var data in datas)
            {
                Datas.TryAdd(data.item_type_code, data);
            }
        }
    }



}
