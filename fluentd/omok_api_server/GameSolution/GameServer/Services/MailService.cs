using GameServer.Models.GameDb;
using GameServer.Repositories.Interfaces;
using GameServer.Services.Interfaces;
using GameShared.DTO;
using ServerShared.Repository.Interfaces;
using ServerShared.ServerCore;

namespace GameServer.Services;

public class MailService : BaseLogger<MailService>, IMailService
{
	private readonly IGameDb<Mail> _mailDb;
	private readonly IItemService _itemService;
	private readonly IMasterDb _masterDb;
	public MailService(ILogger<MailService> logger, IGameDb<Mail> mailDb, IItemService itemService, IMasterDb masterDb) : base(logger)
	{
		_mailDb = mailDb;
		_itemService = itemService;
		_masterDb = masterDb;
	}

	public async Task<(ErrorCode, IEnumerable<MailInfo>?)> GetMailList(Int64 uid)
	{
		try
		{
			var (errorCode, rows) = await _mailDb.GetAll(uid);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return (ErrorCode.MailGetFail, null);
			}

			var list = rows?.Select(mail =>
			{
				if (mail.ExpireDt < DateTime.Now)
				{
					mail.StatusCode = MailStatusCode.Expired;
				}

				if (mail.SenderUid > 0)
				{
					mail.MailType = MailType.User;
				}

				return mail.ToDTO();
			});

			return (ErrorCode.None, list);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.MailGetException, null);
		}
	}

	public async Task<ErrorCode> SendMail(Int64 sendUid, Int64 receiveUid, string title, string content)
	{
		try
		{
			var mail = new Mail
			{
				SenderUid = sendUid,
				ReceiverUid = receiveUid,
				Title = title,
				Content = content,
				ExpireDt = DateTime.Now.AddDays(7),
				RewardCode = 0,
				StatusCode = MailStatusCode.Unread,
				MailType = MailType.System,
			};

			var errorCode = await _mailDb.Set(mail);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return ErrorCode.MailSendFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.MailSendException;
		}
	}

	public async Task<ErrorCode> SendMail(Mail mail)
	{
		try
		{
			var errorCode = await _mailDb.Set(mail);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return ErrorCode.MailSendFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.MailSendException;
		}
	}

	public async Task<ErrorCode> DeleteMail(Int64 uid, Int64 mailUid)
	{
		try
		{
			var (result, mail) = await _mailDb.Get(mailUid);

			if (mail?.ReceiverUid != uid)
			{
				return ErrorCode.MailDeleteFailNotAllowed;
			}

			result = await _mailDb.Delete(mailUid);

			if (result != ErrorCode.None)
			{
				ErrorLog(result);
				return ErrorCode.MailDeleteFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.MailDeleteException;
		}
	}

	public async Task<ErrorCode> ReadMail(Int64 uid, Int64 mailUid)
	{
		try
		{
			var (errorCode, mail) = await _mailDb.Get(mailUid);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return ErrorCode.MailReadFail;
			}

			if (mail?.ReceiverUid != uid)
			{
				return ErrorCode.MailReadFailNotAllowed;
			}

			if (mail.StatusCode == MailStatusCode.Read)
			{
				return ErrorCode.MailReadFailAlreadyRead;
			}

			errorCode = await UpdateMailStatus(mailUid, MailStatusCode.Read);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return errorCode;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.MailReadException;
		}
	}

	public async Task<ErrorCode> ReceiveMail(Int64 uid, Int64 mailUid)
	{
		try
		{
			var (errorCode, mail) = await _mailDb.Get(mailUid);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return ErrorCode.MailReceiveFail;
			}

			if (mail?.ReceiverUid != uid)
			{
				return ErrorCode.MailReceiveFailNotAllowed;
			}

			if (mail.StatusCode == MailStatusCode.Received)
			{
				return ErrorCode.MailReceiveFailAlreadyReceived;
			}

			if (mail.ExpireDt < DateTime.Now)
			{
				return ErrorCode.MailReceiveFailMailExpired;
			}

			if (false == await InsertItems(mail.ReceiverUid, mail.RewardCode))
			{
				return ErrorCode.MailReceiveFailItemInsertFail;
			}

			errorCode = await UpdateMailStatus(mailUid, MailStatusCode.Received);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return errorCode;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.MailReceiveException;
		}
	}

	private async Task<ErrorCode> UpdateMailStatus(Int64 mailUid, MailStatusCode statusCode)
	{
		try
		{
			var errorCode = await _mailDb.Update(mailUid, new { status_code = statusCode });

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return ErrorCode.MailUpdateStatusFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.MailUpdateStatusException;
		}
	}

	private async Task<bool> InsertItems(Int64 uid, int rewardCode)
	{

		var errorCode = ErrorCode.None;
		var items = _masterDb.GetItemsByRewardCode(rewardCode);

		foreach (var (item, count) in items)
		{
			if (null == item)
			{
				continue;
			}

			errorCode = await _itemService.InsertUserItem(uid, item.ItemId, count);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return false;
			}
		}

		return true;
	}
}
