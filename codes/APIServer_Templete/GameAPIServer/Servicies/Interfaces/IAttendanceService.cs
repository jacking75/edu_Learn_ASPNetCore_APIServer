using APIServer.Models;
using APIServer.Models.GameDB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces
{
    public interface IAttendanceService
    {
        public Task<(ErrorCode, GdbAttendanceInfo)> GetAttendanceInfo(int uid);
        public Task<(ErrorCode, List<ReceivedReward>)> CheckAttendanceAndReceiveRewards(int uid);
    }
}
