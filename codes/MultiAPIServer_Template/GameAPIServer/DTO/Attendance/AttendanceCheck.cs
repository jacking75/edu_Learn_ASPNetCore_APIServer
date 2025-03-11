using APIServer.Models;
using System.Collections.Generic;

namespace APIServer.DTO.Attendance
{
    public class AttendanceCheckResponse : ErrorCodeDTO
    {
        public List<ReceivedReward> Rewards { get; set; }
    }
}
