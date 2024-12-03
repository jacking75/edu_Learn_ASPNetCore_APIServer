namespace GameClient;

public class UserDisplayData
{
	public string Nickname { get; set; }
	public Int64 Uid { get; set; }
}

public class MailDisplayData
{
	public MailInfo MailInfo { get; set; }
	public List<(Item, int)> Items { get; set; }
}