using GameAPIServer.Models;
using GameAPIServer.Models.DAO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces;

public interface IAttendanceService
{
    public Task<(ErrorCode, GdbAttendanceInfo)> GetAttendanceInfo(int uid);
    public Task<(ErrorCode, List<ReceivedReward>)> CheckAttendanceAndReceiveRewards(int uid);
}
