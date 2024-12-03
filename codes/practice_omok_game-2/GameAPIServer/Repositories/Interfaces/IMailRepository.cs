using GameAPIServer.Models.GameDb;

namespace GameAPIServer.Repositories.Interfaces;

public interface IMailRepository : IDisposable
{
	public Task<ErrorCode> InsertMail(Mail mail);

	public Task<IEnumerable<MailInfo>?> GetReceivedMails(Int64 uid);

	public Task<MailInfo?> GetMail(Int64 uid, Int64 mailUid);

	public Task<ErrorCode> DeleteMail(Int64 uid, Int64 mailUid);

	public Task<ErrorCode> UpdateMailStatus(Int64 mailUid, MailStatusCode statusCode);
}
