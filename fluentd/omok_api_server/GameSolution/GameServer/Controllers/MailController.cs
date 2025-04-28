using GameServer.Services.Interfaces;
using GameShared.DTO;
using Microsoft.AspNetCore.Mvc;
using ServerShared.ServerCore;

namespace GameServer.Controllers;

[Route("[controller]")]
[ApiController]
public class MailController : BaseController<MailController>
{
	private readonly IMailService _mailService;
	public MailController(ILogger<MailController> logger, IMailService mailService) : base(logger)
	{
		_mailService = mailService;
	}

	[HttpPost("send")]
	public async Task<MailSendResponse> SendMail(MailSendRequest request)
	{
		MailSendResponse response = new();

		response.Result = await _mailService.SendMail(GetUserUid(), request.ReceiverUid, request.Title, request.Content);

		if (ErrorCode.None != response.Result)
		{
			ErrorLog(response.Result, request);
			return response;
		}

		return response;
	}

	[HttpPost("list")]
	public async Task<MailListResponse> GetMailList()
	{
		MailListResponse response = new();

		Int64 uid = GetUserUid();

		(response.Result, response.MailList) = await _mailService.GetMailList(uid);

		if (ErrorCode.None != response.Result)
		{
			ErrorLog(response.Result, uid);
			return response;
		}

		return response;
	}

	[HttpPost("read")]
	public async Task<MailReadResponse> ReadMail(MailReadRequest request)
	{
		MailReadResponse response = new();

		Int64 uid = GetUserUid();

		response.Result = await _mailService.ReadMail(uid, request.MailUid);

		if (ErrorCode.None != response.Result)
		{
			ErrorLog(response.Result, request);
			return response;
		}

		return response;
	}

	[HttpPost("delete")]
	public async Task<MailDeleteResponse> DeleteMail(MailDeleteRequest request)
	{
		MailDeleteResponse response = new();

		response.Result = await _mailService.DeleteMail(GetUserUid(), request.MailUid);

		if (ErrorCode.None != response.Result)
		{
			ErrorLog(response.Result, request);
			return response;
		}

		return response;
	}

	[HttpPost("receive")]
	public async Task<MailReceiveResponse> ReceiveMail(MailReceiveRequest request)
	{
		MailReceiveResponse response = new();

		response.Result = await _mailService.ReceiveMail(GetUserUid(), request.MailUid);

		if (ErrorCode.None != response.Result)
		{
			ErrorLog(response.Result, request);
			return response;
		}

		return response;
	}
}
