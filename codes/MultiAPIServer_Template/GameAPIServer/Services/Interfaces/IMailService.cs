using GameAPIServer.DTO.DataLoad;
using GameAPIServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces
{
    public interface IMailService
    {
        public Task<(ErrorCode, List<UserMailInfo>)> GetMailList(int uid);
        public Task<(ErrorCode, List<ReceivedReward>)> ReceiveMail(int uid, int mailSeq);
        public Task<ErrorCode> DeleteMail(int uid, int mailSeq);
    }
}
