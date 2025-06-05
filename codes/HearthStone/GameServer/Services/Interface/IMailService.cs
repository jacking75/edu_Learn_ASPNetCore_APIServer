using GameServer.Models;
namespace GameServer.Services.Interface;

public interface IMailService
{
    public Task<(ErrorCode, List<MailInfo>)> GetMailInfoList(Int64 accountUid);
    public Task<ErrorCode> ReadMail(Int64 accountUid, Int64 mailId);
    public Task<ErrorCode> DeleteMail(Int64 accountUid, Int64 mailId);

}
