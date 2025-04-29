using HiveAPIServer.Models;
using System.Collections.Generic;

namespace HiveAPIServer.DTO.Attendance
{
    public class AttendanceCheckResponse : ErrorCodeDTO
    {
        public List<ReceivedReward> Rewards { get; set; }
    }
}
