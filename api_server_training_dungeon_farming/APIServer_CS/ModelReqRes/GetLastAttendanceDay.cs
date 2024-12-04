using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelReqRes;

public class GetAttendanceBookRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH_TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

}

public class GetAttendanceBookResponse : ResponseBase
{
    [Required]
    [Range(0, Int16.MaxValue, ErrorMessage = "LAST_ATTENDANCE_DAY IS NOT VALID")]
    public Int16 LastAttendanceDay { get; set; } = 0;
}