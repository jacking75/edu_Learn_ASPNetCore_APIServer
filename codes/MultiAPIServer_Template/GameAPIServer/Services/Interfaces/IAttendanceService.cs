using HiveAPIServer.Models;
using HiveAPIServer.Models.GameDB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiveAPIServer.Servicies.Interfaces
{
    public interface IAttendanceService
    {
        public Task<(ErrorCode, GdbAttendanceInfo)> GetAttendanceInfo(int uid);
        public Task<(ErrorCode, List<ReceivedReward>)> CheckAttendanceAndReceiveRewards(int uid);
    }
}
