namespace GameAPIServer.Models.GameDb;

public class GameResult
{
	public Int64 game_result_uid { get; set; }
	public Int64 black_user_uid{ get; set; }
	public Int64 white_user_uid{ get; set; }
	public DateTime start_dt { get; set; }
	public DateTime end_dt { get; set; }
	public int result_code { get; set; }
}
