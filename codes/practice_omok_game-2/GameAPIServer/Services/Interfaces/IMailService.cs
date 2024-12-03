using GameAPIServer.Models.MasterDb;

namespace GameAPIServer.Services.Interfaces;

public interface IMailService
{
	public Task<ErrorCode> SendReward(Int64 uid, int rewardCode, string title = "System Reward");

	public Task<ErrorCode> SendMail(Int64 sendUid, Int64 receiveUid, string title, string content);

	public Task<(ErrorCode, IEnumerable<MailInfo>?)> GetMails(Int64 uid);

	public Task<(ErrorCode, MailInfo?, List<(Item, int)>?)> ReadMail(Int64 uid, Int64 mailUid);

	public Task<ErrorCode> DeleteMail(Int64 uid, Int64 mailUid);

	public Task<ErrorCode> ReceiveRewardFromMail(Int64 uid, Int64 mailUid);
}
