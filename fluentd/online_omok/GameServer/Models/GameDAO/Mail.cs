
namespace GameServer.Models.GameDb;

public class Mail 
{
	public Int64 Uid { get; set; }
	public Int64 ReceiverUid { get; set; }
	public Int64 SenderUid { get; set; }
	public string Title { get; set; } = "";
	public string Content { get; set; } = "";
	public MailStatusCode StatusCode { get; set; } = 0;
	public int RewardCode { get; set; } = 0;
	public DateTime CreateDt { get; set; }
	public DateTime UpdateDt { get; set; }
	public DateTime ExpireDt { get; set; }
	public MailType MailType { get; set; }

	public static readonly string Table = "mail";
	public static readonly string[] SelectColumns =
	{
		"uid as Uid",
		"receiver_uid as ReceiverUid",
		"sender_uid as SenderUid",
		"title as Title",
		"content as Content",
		"status_code as StatusCode",
		"reward_code as RewardCode",
		"create_dt as CreateDt",
		"update_dt as UpdateDt",
		"expire_dt as ExpireDt",
	};

	public GameShared.DTO.MailInfo ToDTO()
	{
		return new GameShared.DTO.MailInfo
		{
			Uid = this.Uid,
			ReceiverUid = this.ReceiverUid,
			SenderUid = this.SenderUid,
			Title = this.Title,
			Content = this.Content,
			StatusCode = this.StatusCode,
			RewardCode = this.RewardCode,
			CreateDt = this.CreateDt,
			UpdateDt = this.UpdateDt,
			ExpireDt = this.ExpireDt,
		};
	}
}