using System.Collections.Generic;

namespace GameAPIServer.Models.DTO;

public class AttendanceCheckResponse : ErrorCode
{
    public List<ReceivedReward> Rewards { get; set; }
}
