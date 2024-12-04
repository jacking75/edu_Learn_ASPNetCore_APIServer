using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;

using SqlKata.Execution;

namespace APIServer.Services.MasterData.Storages
{
    public class AttendanceRewardStorage
    {
        public Dictionary<Int32, AttendanceReward> Datas { get; private set; } = null;

        private readonly string _tableName = "attendance_event_reward";

        public async Task<bool> Load(QueryFactory queryFactory)
        {
            var loadedDatas = await MasterDataLoader.LoadAsList<AttendanceReward>(queryFactory, _tableName);

            if (loadedDatas is null)
            {
                return false;
            }

            Store(loadedDatas);

            return true;
        }

        private void Store(List<AttendanceReward> datas)
        {
            Datas = new Dictionary<Int32, AttendanceReward>(datas.Count);
            foreach (var data in datas)
            {
                Datas.TryAdd(data.days, data);
            }
        }



    }



}
