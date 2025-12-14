using GameAPIServer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GameAPIServer.Repository.Interfaces;

public interface IGameDb
{
    public Task<(ErrorCode, Int64)> CreateAccount(string userID, string pw);

    public Task<(ErrorCode, Int64)> VerifyUser(string userID, string pw);


    public Task<int> InsertInitMoneyInfo(Int64 uid, IDbTransaction transaction);
    public Task<int> InsertInitAttendance(Int64 uid, IDbTransaction transaction);
  
    
    public Task<IEnumerable<GdbMailboxInfo>> GetMailList(Int64 uid);
    public Task<GdbMailboxInfo> GetMailInfo(int mailSeq);
    public Task<IEnumerable<GdbMailboxRewardInfo>> GetMailRewardList(int mailSeq);
    public Task<int> DeleteMail(int mailSeq);
    public Task<int> DeleteMailReward(int mailSeq);
    public Task<int> UpdateReceiveMail(int mailSeq);
    public Task<int> UpdateMainChar(Int64 uid, int charKey);

    public Task<GdbUserMoneyInfo> GetUserMoneyById(Int64 uid);
    
    
    public Task<IEnumerable<RdbUserScoreData>> SelectAllUserScore();

    public Task<GdbAttendanceInfo> GetAttendanceById(Int64 uid);
    public Task<int> CheckAttendanceById(Int64 uid);
    public Task<int> UpdateUserjewelry(Int64 uid, int rewardQty);
    public IDbConnection GDbConnection();
}