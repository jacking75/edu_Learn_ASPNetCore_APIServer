using System.Collections.Generic;
using GameAPIServer.Models;

namespace GameAPIServer.DTOs;

public class AttendanceCheckResponse : ErrorCode
{
    public List<ReceivedReward> Rewards { get; set; }
}
