using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;

using SqlKata.Execution;

namespace APIServer.Services.MasterData.Storages
{
    public class StageEnemyStorage
    {
        public Dictionary<Int32, List<StageEnemy>> Datas { get; private set; } = new();

        private readonly string _tableName = "stage_enemy";

        public async Task<bool> Load(QueryFactory queryFactory)
        {
            var loadedDatas = await MasterDataLoader.LoadAsList<StageEnemy>(queryFactory, _tableName);

            if (loadedDatas is null)
            {
                return false;
            }

            Store(loadedDatas);

            return true;
        }

        private void Store(List<StageEnemy> datas)
        {
            foreach (var data in datas)
            {
                if (Datas.ContainsKey(data.stage_code) == false)
                {
                    Datas.Add(data.stage_code, new List<StageEnemy>());
                }

                Datas[data.stage_code].Add(data);
            }
        }
    }



}
