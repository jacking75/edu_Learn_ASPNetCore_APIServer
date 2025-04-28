using GameServer.Models.GameDb;
using GameShared.DTO;

namespace GameServer.Services.Interfaces;

public interface IMailService
{
    public Task<(ErrorCode, IEnumerable<MailInfo>?)> GetMailList(Int64 uid);
    public Task<ErrorCode> SendMail(Int64 sendUid, Int64 receiveUid, string Title, string Content);
    public Task<ErrorCode> SendMail(Mail mail);
    public Task<ErrorCode> DeleteMail(Int64 uid, Int64 mailUid);
    public Task<ErrorCode> ReadMail(Int64 uid, Int64 mailUid);
    public Task<ErrorCode> ReceiveMail(Int64 uid, Int64 mailUid);
}
