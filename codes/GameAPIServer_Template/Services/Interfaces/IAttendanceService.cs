using GameAPIServer.DTOs;
using GameAPIServer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces;

public interface IAttendanceService
{
    public Task<(ErrorCode, GdbAttendanceInfo)> GetAttendanceInfo(Int64 uid);
    public Task<(ErrorCode, List<ReceivedReward>)> CheckAttendanceAndReceiveRewards(Int64 uid);
}
