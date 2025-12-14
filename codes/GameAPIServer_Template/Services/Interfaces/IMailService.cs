using GameAPIServer.DTOs;
using GameAPIServer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces;

public interface IMailService
{
    public Task<(ErrorCode, List<UserMailInfo>)> GetMailList(Int64 uid);
    public Task<(ErrorCode, List<ReceivedReward>)> ReceiveMail(Int64 uid, int mailSeq);
    public Task<ErrorCode> DeleteMail(Int64 uid, int mailSeq);
}
