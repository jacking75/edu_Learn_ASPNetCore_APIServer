using GameAPIServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class Mail : SecureController<Mail>
{
	private readonly IMailService _mailService;

	public Mail(ILogger<Mail> logger, IMailService MailService) : base(logger)
	{
		_mailService = MailService;
	}

	[HttpPost("check")]
	public async Task<GetMailResponse> GetMail()
	{
		GetMailResponse response = new();

		Int64 uid = GetUserUid();

		(response.Result, response.MailData) = await _mailService.GetMails(uid);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[CheckMail] Error: {response.Result}");
		}

		return response;
	}

	[HttpPost("receive")]
	public async Task<ReceiveMailResponse> ReceiveMailReward([FromBody] ReceiveMailRequest request)
	{
		ReceiveMailResponse response = new();

		Int64 uid = GetUserUid();

		(response.Result) = await _mailService.ReceiveRewardFromMail(uid, request.MailUid);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[CheckMail] Error: {response.Result}");
		}

		return response;
	}


	[HttpPost("read")]
	public async Task<ReadMailResponse> ReadMail([FromBody] ReadMailRequest request)
	{
		ReadMailResponse response = new();

		Int64 uid = GetUserUid();

		(response.Result, response.MailInfo, response.Items) = await _mailService.ReadMail(uid, request.MailUid);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[CheckMail] Error: {response.Result}");
		}

		return response;
	}

	[HttpPost("send")]
	public async Task<SendMailResponse> SendMail([FromBody] SendMailRequest request)
	{
		SendMailResponse response = new();

		Int64 uid = GetUserUid();

		response.Result = await _mailService.SendMail(uid, request.ReceiveUid, request.Title, request.Content);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[CheckMail] Error: {response.Result}");
		}

		return response;
	}


	[HttpPost("delete")]
	public async Task<DeleteMailResponse> DeleteMail([FromBody] DeleteMailRequest request)
	{
		DeleteMailResponse response = new();

		Int64 uid = GetUserUid();

		response.Result = await _mailService.DeleteMail(uid, request.MailUid);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[CheckMail] Error: {response.Result}");
		}

		return response;
	}
}
