namespace GameAPIServer.Models.GameDb;


public class Mail
{
    public Int64 mail_uid { get; set; }
    public Int64 receive_user_uid{ get; set; }
	public Int64 send_user_uid{ get; set; }
	public string mail_title { get; set; } = "";
    public string mail_content { get; set; } = "";
    public int mail_status_code { get; set; } = 0;
    public int reward_code { get; set; } = 0;
    public DateTime create_dt { get; set; }
    public DateTime update_dt { get; set; }
    public DateTime expire_dt { get; set; }

	public static string[] SelectColumns =
	{
		"mail_uid as MailUid",
		"receive_user_uid as ReceiveUid",
		"send_user_uid as SendUid",
		"mail_title as Title",
		"mail_content as Content",
		"mail_status_code as StatusCode",
		"reward_code as RewardCode",
		"create_dt as CreateDateTime",
		"update_dt as UpdateDateTime",
		"expire_dt as ExpireDateTime",
	};
}
