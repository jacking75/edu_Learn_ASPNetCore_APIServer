using GameServer.Models;
using GameServer.Models.DTO;

namespace GameServer.Services.Interface;

public interface IAttendanceService
{
    public Task<(ErrorCode, List<AttendanceInfo>)> GetAttendanceInfoList(Int64 accountUid);
    public Task<(ErrorCode, ReceivedReward?)> CheckAttendanceAndReceiveRewards(Int64 accountUid, int eventKey);
}
