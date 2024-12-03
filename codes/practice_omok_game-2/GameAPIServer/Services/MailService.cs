using GameAPIServer.Models.GameDb;
using GameAPIServer.Services.Interfaces;
using GameAPIServer.Repositories.Interfaces;
using ZLogger;

namespace GameAPIServer.Services;

public class MailService : IMailService
{
	private readonly ILogger<MailService> _logger;
	private readonly IMailRepository _mailDb;
	private readonly IMasterRepository _masterDb;
	private readonly IItemService _itemService;

	public MailService(ILogger<MailService> logger, IMailRepository postGameRepository, IItemService itemService, IMasterRepository masterDb)
	{
		_logger = logger;
		_mailDb = postGameRepository;
		_itemService = itemService;
		_masterDb = masterDb;
	}

	public async Task<(ErrorCode, IEnumerable<MailInfo>?)> GetMails(Int64 uid)
	{
		try
		{
			var mails = await _mailDb.GetReceivedMails(uid);

			if (mails == null)
			{
				_logger.ZLogError($"[GetMails] Uid: {uid}");
				return (ErrorCode.MailGetFail, null);
			}

			var list = mails.Select((mailInfo) =>
			{
				if (mailInfo.ExpireDateTime < DateTime.Now)
				{
					mailInfo.StatusCode = MailStatusCode.Expired;
				}

				if (mailInfo.SendUid > 0)
				{
					mailInfo.Type = MailType.User;
				}

				return mailInfo;
			});

			return (ErrorCode.None, list);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetMails] ErrorCode: {ErrorCode.MailGetException}, Uid: {uid}");
			return (ErrorCode.MailGetException, null);
		}

	}

	public async Task<ErrorCode> SendReward(Int64 uid, int rewardCode, string title = "System Reward")
	{
		try
		{
			var mail = new Mail
			{
				mail_title = title,
				receive_user_uid = uid,
				reward_code = rewardCode,
				expire_dt = DateTime.Now.AddDays(7)
			};

			return await _mailDb.InsertMail(mail);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[SendReward] ErrorCode: {ErrorCode.MailSendRewardException}, Uid: {uid}");
			return ErrorCode.MailSendRewardException;
		}

	}

	public async Task<ErrorCode> DeleteMail(Int64 uid, Int64 mailUid)
	{
		try
		{
			return await _mailDb.DeleteMail(uid, mailUid);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[DeletesMail] ErrorCode: {ErrorCode.MailDeleteException}, Uid: {uid}, MailUid: {mailUid}");
			return ErrorCode.MailDeleteException;
		}
	}

	public async Task<ErrorCode> SendMail(Int64 sendUid, Int64 receiveUid, string title, string content)
	{
		try
		{
			var mail = new Mail
			{
				mail_title = title,
				mail_content = content,
				send_user_uid = sendUid,
				receive_user_uid = receiveUid,
				expire_dt = DateTime.Now.AddDays(7)
			};

			return await _mailDb.InsertMail(mail);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[SendReward] ErrorCode: {ErrorCode.MailSendRewardException}, Uid: {sendUid}");
			return ErrorCode.MailSendRewardException;
		}

	}

	public async Task<ErrorCode> ReceiveRewardFromMail(Int64 uid, Int64 mailUid)
	{
		try
		{
			var (result, mail, items) = await GetMailDetails(uid, mailUid);

			if (ErrorCode.None != result)
			{
				_logger.ZLogError($"[ReceiveReward] ErrorCode: {result}, Uid: {uid}, MailUid: {mailUid}");
				return result;
			}

			result = CheckMailAvailability(mail, items);

			if (ErrorCode.None != result)
			{
				return result;
			}

			if (false == await InsertItems(uid, items))
			{
				_logger.ZLogError($"[ReceiveReward] ErrorCode: {result}, Uid: {uid}, MailUid: {mailUid}");
				return result;
			}

			result = await _mailDb.UpdateMailStatus(mailUid, MailStatusCode.Received);

			if (ErrorCode.None != result)
			{
				_logger.ZLogError($"[ReceiveReward] ErrorCode: {result}, Uid: {uid}, MailUid: {mailUid}");
			}

			return result;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[ReceiveReward] ErrorCode: {ErrorCode.MailReceiveException}, Uid: {uid}, MailUid: {mailUid}");
			return ErrorCode.MailReceiveException;
		}
	}

	public async Task<(ErrorCode, MailInfo?, List<(Item, int)>?)> ReadMail(Int64 uid, Int64 mailUid)
	{
		try
		{
			var (result, mail, items) = await GetMailDetails(uid, mailUid);

			if (ErrorCode.None != result				
			&& ErrorCode.MailReceiveFailRewardNotFound != result) 
			{
				_logger.ZLogError($"[ReadMail] ErrorCode: {result}, Uid: {uid}, MailUid: {mailUid}");
				return (result, null, null);
			}

			result = await _mailDb.UpdateMailStatus(mailUid, MailStatusCode.Read);

			if (ErrorCode.None != result)
			{
				_logger.ZLogError($"[ReadMail] ErrorCode: {result}, Uid: {uid}, MailUid: {mailUid}");
			}

			return (result, mail, items);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[ReadMail] ErrorCode: {ErrorCode.MailReadException}, Uid: {uid}, MailUid: {mailUid}");
			return (ErrorCode.MailReadException, null, null);
		}
	}

	private async Task<(ErrorCode, MailInfo?, List<(Item, int)>?)> GetMailDetails(Int64 uid, Int64 mailUid)
	{

		MailInfo? mail = await _mailDb.GetMail(uid, mailUid);

		if (null == mail)
		{
			return (ErrorCode.MailReceiveFailMailNotFound, mail, null);
		}

		if (mail.ExpireDateTime < DateTime.Now)
		{
			return (ErrorCode.MailGetFailMailExpired, mail, null);
		}


		if (mail.RewardCode == 0)
		{
			return (ErrorCode.MailReceiveFailRewardNotFound, mail, null);
		}

		List<(Item, int)> items = _masterDb.GetRewardItems(mail.RewardCode);

		return (ErrorCode.None, mail, items);
	}

	private async Task<bool> InsertItems(Int64 uid, List<(Item, int)> items)
	{
		foreach (var (item, count) in items)
		{
			if (null == item)
				continue;
			
			if (ErrorCode.None != await _itemService.InsertUserItem(uid, item.ItemId, count))
			{
				_logger.ZLogError($"[InsertItem] Uid: {uid} ItemId:{item.ItemId}");
				return false;
			}
		}
		return true;
	}

	private ErrorCode CheckMailAvailability(MailInfo? mail, List<(Item, int)>? items)
	{

		if (null == items || items.Count == 0)
		{
			return ErrorCode.MailReceiveFailRewardNotFound;
		}

		if (null == mail)
		{
			return ErrorCode.MailReceiveFailMailNotFound;
		}

		if (mail.StatusCode == MailStatusCode.Received)
		{
			return ErrorCode.MailReceiveFailAlreadyReceived;
		}

		if (mail.StatusCode == MailStatusCode.Expired)
		{
			return ErrorCode.MailReceiveFailExpired;
		}

		return ErrorCode.None;
	}
}
