using APIServer.Models;
using System.Collections.Generic;

namespace GameAPIServer.DTO
{
    public class AttendanceCheckResponse : ErrorCodeDTO
    {
        public List<ReceivedReward> Rewards { get; set; }
    }
}
