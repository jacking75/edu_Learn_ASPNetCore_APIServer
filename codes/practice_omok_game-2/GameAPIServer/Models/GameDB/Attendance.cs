namespace GameAPIServer.Models.GameDb;

public class UserAttendance 
{
	public Int64 user_uid{ get; set; }
	public int attendance_code { get; set; }
	public int attendance_seq { get; set; }


	public static string[] SelectColumns =
	[
		"user_uid as Uid",
		"attendance_code as AttendanceCode",
		"attendance_seq as AttendanceCount",
	];
}
