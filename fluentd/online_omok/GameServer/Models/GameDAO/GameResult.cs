namespace GameServer.Models.GameDb;

public class GameResult
{
	public Int64 Uid { get; set; }
	public Int64 BlackUserUid { get; set; }
	public Int64 WhiteUserUid { get; set; }
	public DateTime StartDt { get; set; }
	public DateTime EndDt { get; set; }
	public OmokResultCode ResultCode { get; set; }

	public static readonly string Table = "game_result";
	public static readonly string[] SelectColumns =
	{
		"uid as Uid",
		"black_user_uid as BlackUserUid",
		"white_user_uid as WhiteUserUid",
		"start_dt as StartDt",
		"end_dt as EndDt",
		"result_code as ResultCode",
	};
}
