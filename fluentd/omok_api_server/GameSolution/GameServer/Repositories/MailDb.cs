using GameServer.Models.GameDb;
using Microsoft.Extensions.Options;
using ServerShared.Repository;
using SqlKata.Execution;

namespace GameServer.Repositories;

public class MailDb : GameDb<Mail>
{
	public MailDb(ILogger<Mail> logger, IOptions<ServerConfig> dbConfig) : base(logger, dbConfig)
	{
	}

	public override async Task<ErrorCode> Set(Mail mail)
	{
		try
		{
			var count = await _queryFactory.Query(Mail.Table)
				.InsertAsync(new {
					receiver_uid = mail.ReceiverUid,
					sender_uid = mail.SenderUid,
					title = mail.Title,
					content = mail.Content,
					status_code = mail.StatusCode,
					reward_code = mail.RewardCode,
					create_dt = mail.CreateDt,
					update_dt = mail.UpdateDt,
					expire_dt = mail.ExpireDt,
				});

			if (count < 1)
			{
				return ErrorCode.DbMailInsertFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.DbMailInsertException;
		}
	}

	public override async Task<ErrorCode> Update(Int64 uid, object value)
	{
		try
		{
			var count = await _queryFactory.Query(Mail.Table)
				.Where("uid", uid)
				.UpdateAsync(value);

			if (count < 1)
			{
				return ErrorCode.DbMailUpdateFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.DbMailUpdateException;
		}
	}

	public override async Task<ErrorCode> Delete(Int64 mailUid)
	{
		try
		{
			var count = await _queryFactory.Query(Mail.Table)
				.Where("uid", mailUid)
				.DeleteAsync();

			if (count < 1)
			{
				return ErrorCode.DbMailDeleteFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.DbMailDeleteException;
		}
	}

	public override async Task<(ErrorCode, Mail?)> Get(Int64 mailUid)
	{
		try
		{
			var mail = await _queryFactory.Query(Mail.Table)
				.Where("uid", mailUid)
				.Select(Mail.SelectColumns)
				.FirstOrDefaultAsync<Mail>();

			if (mail == null)
			{
				return (ErrorCode.DbMailGetFail, null);
			}

			return (ErrorCode.None, mail);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.DbMailGetException, null);
		}
	}

	public override async Task<(ErrorCode, IEnumerable<Mail>?)> GetAll(Int64 uid)
	{
		try
		{
			var mails = await _queryFactory.Query(Mail.Table)
				.Where("receiver_uid", uid)
				.Select(Mail.SelectColumns)
				.GetAsync<Mail>();

			if (mails == null)
			{
				return (ErrorCode.DbMailGetFail, null);
			}

			return (ErrorCode.None, mails);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.DbMailGetException, null);
		}
	}
}
