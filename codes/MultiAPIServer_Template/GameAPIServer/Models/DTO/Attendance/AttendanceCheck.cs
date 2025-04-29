using MatchAPIServer.Models;
using System.Collections.Generic;

namespace MatchAPIServer.DTO.Attendance
{
    public class AttendanceCheckResponse : ErrorCodeDTO
    {
        public List<ReceivedReward> Rewards { get; set; }
    }
}
