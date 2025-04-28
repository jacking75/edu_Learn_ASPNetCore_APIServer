using System.ComponentModel.DataAnnotations;

namespace GameShared.DTO;

public class MailListRequest
{
	public int Page { get; set; } = 0;
}

public class MailListResponse : ErrorCodeDTO
{
	public IEnumerable<MailInfo>? MailList { get; set; }
}

public class MailReadRequest
{
	[Required]
	public Int64 MailUid { get; set; }
}

public class MailReadResponse : ErrorCodeDTO;

public class MailReceiveRequest
{
	[Required]
	public Int64 MailUid { get; set; }
}

public class MailReceiveResponse : ErrorCodeDTO;

public class MailDeleteRequest
{
	[Required]
	public Int64 MailUid { get; set; }
}

public class MailDeleteResponse : ErrorCodeDTO;

public class MailSendRequest
{
	[Required]
	public Int64 ReceiverUid { get; set; }
	[Required]
	public string Title { get; set; } = "";
	[Required]
	public string Content { get; set; } = "";
}

public class MailSendResponse : ErrorCodeDTO;
