using GameAPIServer.Models;
using System.Collections.Generic;

namespace GameAPIServer.DTO.Attendance
{
    public class AttendanceCheckResponse : ErrorCodeDTO
    {
        public List<ReceivedReward> Rewards { get; set; }
    }
}
