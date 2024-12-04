using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;

using SqlKata.Execution;

namespace APIServer.Services.MasterData.Storages
{
    public class InAppProductStorage
    {
        public Dictionary<Int32, List<InAppProduct>> Datas { get; private set; } = new();

        private readonly string _tableName = "inapp_product";

        public async Task<bool> Load(QueryFactory queryFactory)
        {
            var loadedDatas = await MasterDataLoader.LoadAsList<InAppProduct>(queryFactory, _tableName);

            if (loadedDatas is null)
            {
                return false;
            }

            Store(loadedDatas);

            return true;
        }

        private void Store(List<InAppProduct> datas)
        {
            foreach (var data in datas)
            {
                if (Datas.ContainsKey(data.pid) == false)
                {
                    Datas.Add(data.pid, new List<InAppProduct>());
                }

                Datas[data.pid].Add(data);
            }
        }
    }



}
