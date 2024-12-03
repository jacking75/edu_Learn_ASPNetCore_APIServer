using System.Data;
using GameAPIServer.Models.GameDb;
using GameAPIServer.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;

namespace GameAPIServer.Repositories;

public class MailRepository : IMailRepository
{
	readonly ILogger<MailRepository> _logger;
	readonly IOptions<ServerConfig> _dbConfig;

	IDbConnection _dbConn;
	SqlKata.Compilers.MySqlCompiler _compiler;
	QueryFactory _queryFactory;

	public MailRepository(ILogger<MailRepository> logger, IOptions<ServerConfig> dbConfig)
	{
		_dbConfig = dbConfig;
		_logger = logger;

		Open();

		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, _compiler);
	}

	public async Task<ErrorCode> InsertMail(Mail mail)
	{
		try
		{
			var result = await _queryFactory.Query("mail")
								   .InsertGetIdAsync<int>(new
								   {
									   mail.mail_title,
									   mail.mail_content,
									   mail.receive_user_uid,
									   mail.reward_code,
									   mail.expire_dt,
									   mail.send_user_uid,
									   create_dt = DateTime.Now
								   });

			if (result > 0)
				return ErrorCode.None;

			return ErrorCode.DbMailInsertFail;

		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[InserMail Failed] ErrorMessage: {e.Message}");
			return ErrorCode.DbMailInsertException;
		}
	}

	public async Task<IEnumerable<MailInfo>?> GetReceivedMails(Int64 uid)
	{
		try
		{
			return await _queryFactory.Query("mail").Where("receive_user_uid", uid)
												.Select(Mail.SelectColumns)
												.OrderByDesc("create_dt")
												.GetAsync<MailInfo>();
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetReceivedMails Failed] ErrorMessage: {e.Message}");
			return null;
		}
	}

	public async Task<MailInfo?> GetMail(Int64 uid, Int64 mailUid)
	{
		try 
		{
			return await _queryFactory.Query("mail").Where("mail_uid", mailUid)
											.Where("receive_user_uid", uid)
											.Select(Mail.SelectColumns)
											.FirstAsync<MailInfo>();
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetMail Failed] ErrorMessage: {e.Message}");
			return null;
		}
	}

	public async Task<ErrorCode> DeleteMail(Int64 uid, Int64 MailUid)
	{
		try
		{
			return await _queryFactory.Query("mail")
										.Where("mail_uid", MailUid)
										.Where("receive_user_uid", uid)
										.DeleteAsync() > 0 ? ErrorCode.None : ErrorCode.DbMailDeleteFail;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[DeleteMail Failed] ErrorMessage: {e.Message}");
			return ErrorCode.DbMailDeleteException;
		}
	}

	public async Task<ErrorCode> UpdateMailStatus(Int64 mailUid, MailStatusCode statusCode)
	{
		try
		{
			if (statusCode == MailStatusCode.Read)
			{
				var mail = await _queryFactory.Query("mail").Where("mail_uid", mailUid)
											.Select(Mail.SelectColumns)
											.FirstAsync<MailInfo>();
				if (mail == null)
					return ErrorCode.DbMailUpdateFail;

				if (mail.StatusCode == MailStatusCode.Received ||
					mail.StatusCode == MailStatusCode.Read)
					return ErrorCode.None;
			}

			return await _queryFactory.Query("mail")
										.Where("mail_uid", mailUid)
										.UpdateAsync(new
										{
											mail_status_code = statusCode
										}) > 0 ? ErrorCode.None : ErrorCode.DbMailUpdateFail;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[UpdateMailStatus Failed] ErrorMessage: {e.Message}");
			return ErrorCode.DbMailUpdateException;
		}
	}

	public void Dispose()
	{
		Close();
	}
	void Open()
	{
		_dbConn = new MySqlConnection(_dbConfig.Value.GameDb);
		_dbConn.Open();
	}
	void Close()
	{
		_dbConn.Close();
	}
}
