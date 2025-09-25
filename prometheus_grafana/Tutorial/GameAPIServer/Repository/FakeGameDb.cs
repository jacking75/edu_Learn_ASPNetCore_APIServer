using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GameAPIServer.Models;
using GameAPIServer.Models.DAO;

namespace GameAPIServer.Repository;

public class FakeGameDb : IGameDb
{
    public Task<ErrorCode> CreateAccount(string userID, string pw)
        => Task.FromResult(ErrorCode.None);

    public Task<(ErrorCode, long)> VerifyUser(string userID, string pw)
        => Task.FromResult((ErrorCode.None, 1L));

    public Task<GdbUserInfo> GetUserByPlayerId(long playerId)
        => Task.FromResult(new GdbUserInfo { uid = 1, player_id = "player1", nickname = "FakeUser" });

    public Task<GdbUserInfo> GetUserByUid(int uid)
        => Task.FromResult(new GdbUserInfo { uid = uid, player_id = "player1", nickname = "FakeUser" });

    public Task<GdbUserInfo> GetUserByNickname(string nickname, IDbTransaction transaction)
        => Task.FromResult(new GdbUserInfo { uid = 1, player_id = "player1", nickname = nickname });

    public Task<int> InsertUser(long playerId, string nickname, IDbTransaction transaction)
        => Task.FromResult(1);

    public Task<int> UpdateRecentLogin(int uid)
        => Task.FromResult(1);

    public Task<GdbFriendInfo> GetFriendReqInfo(int uid, int friendUid)
        => Task.FromResult(new GdbFriendInfo { uid = uid, friend_uid = friendUid.ToString(), friend_yn = false, create_dt = DateTime.UtcNow });

    public Task<int> InsertFriendReq(int uid, int friendUid, bool accept = false)
        => Task.FromResult(1);

    public Task<int> InsertFriendReq(int uid, int friendUid, IDbTransaction transaction, bool accept = false)
        => Task.FromResult(1);

    public Task<int> UpdateFriendReqAccept(int uid, int friendUid, IDbTransaction transaction, bool accept = false)
        => Task.FromResult(1);

    public Task<IEnumerable<GdbFriendInfo>> GetFriendInfoList(int uid)
        => Task.FromResult<IEnumerable<GdbFriendInfo>>(new List<GdbFriendInfo>());

    public Task<int> DeleteFriendEachOther(int uid, int friendUid)
        => Task.FromResult(1);

    public Task<int> DeleteFriendReq(int uid, int friendUid)
        => Task.FromResult(1);

    public Task<int> InsertInitMoneyInfo(int uid, IDbTransaction transaction)
        => Task.FromResult(1);

    public Task<int> InsertInitAttendance(int uid, IDbTransaction transaction)
        => Task.FromResult(1);

    public Task<IEnumerable<GdbMailboxInfo>> GetMailList(int uid)
        => Task.FromResult<IEnumerable<GdbMailboxInfo>>(new List<GdbMailboxInfo>());

    public Task<GdbMailboxInfo> GetMailInfo(int mailSeq)
        => Task.FromResult(new GdbMailboxInfo { mail_seq = mailSeq, uid = 1, mail_title = "Test Mail", create_dt = DateTime.UtcNow, expire_dt = DateTime.UtcNow.AddDays(7), receive_dt = DateTime.MinValue, receive_yn = false });

    public Task<IEnumerable<GdbMailboxRewardInfo>> GetMailRewardList(int mailSeq)
        => Task.FromResult<IEnumerable<GdbMailboxRewardInfo>>(new List<GdbMailboxRewardInfo>());

    public Task<int> DeleteMail(int mailSeq)
        => Task.FromResult(1);

    public Task<int> DeleteMailReward(int mailSeq)
        => Task.FromResult(1);

    public Task<int> UpdateReceiveMail(int mailSeq)
        => Task.FromResult(1);

    public Task<GdbAttendanceInfo> GetAttendanceById(int uid)
        => Task.FromResult(new GdbAttendanceInfo { uid = uid, attendance_cnt = 0, recent_attendance_dt = DateTime.UtcNow });

    public Task<GdbUserMoneyInfo> GetUserMoneyById(int uid)
        => Task.FromResult(new GdbUserMoneyInfo { uid = uid, jewelry = 0, gold_medal = 0, sunchip = 0, cash = 0 });

    public Task<int> UpdateMainChar(int uid, int charKey)
        => Task.FromResult(1);

    public Task<IEnumerable<RdbUserScoreData>> SelectAllUserScore()
        => Task.FromResult<IEnumerable<RdbUserScoreData>>(new List<RdbUserScoreData>());

    public Task<int> CheckAttendanceById(int uid)
        => Task.FromResult(1);

    public Task<int> UpdateUserjewelry(int uid, int rewardQty)
        => Task.FromResult(1);

    public IDbConnection GDbConnection()
        => null;
}
