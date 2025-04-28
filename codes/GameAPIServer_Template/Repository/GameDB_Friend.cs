using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GameAPIServer.Repository.Interfaces;
using GameAPIServer.Models.DAO;
using SqlKata.Execution;

namespace GameAPIServer.Repository;

public partial class GameDb : IGameDb
{
    public async Task<GdbFriendInfo> GetFriendReqInfo(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .Where("uid", uid)
                                .Where("friend_uid", friendUid)
                                .FirstOrDefaultAsync<GdbFriendInfo>();
    }

    public async Task<int> InsertFriendReq(int uid, int friendUid, bool accept = false)
    {
        return await _queryFactory.Query("friend")
                                .InsertAsync(new
                                {
                                    uid = uid,
                                    friend_uid = friendUid,
                                    friend_yn = accept,
                                    create_dt = DateTime.Now,
                                });
    }

    public async Task<int> InsertFriendReq(int uid, int friendUid, IDbTransaction transaction, bool accept = false)
    {
        return await _queryFactory.Query("friend")
                                .InsertAsync(new
                                {
                                    uid = uid,
                                    friend_uid = friendUid,
                                    friend_yn = accept,
                                    create_dt = DateTime.Now,
                                }, transaction);
    }

    public async Task<int> UpdateFriendReqAccept(int uid, int friendUid, IDbTransaction transaction, bool accept = false)
    {
        return await _queryFactory.Query("friend").Where("uid", friendUid).Where("friend_uid", uid).UpdateAsync(new
        {
            friend_yn = accept,
        }, transaction);
    }

    public async Task<IEnumerable<GdbFriendInfo>> GetFriendInfoList(int uid)
    {
        return await _queryFactory.Query("friend").Where("friend_uid", uid)
                                                  .Where("friend_yn", false)
                                                  .OrWhere("uid", uid)
                                                  .GetAsync<GdbFriendInfo>();
    }

    public async Task<int> DeleteFriendEachOther(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .WhereRaw($"(uid={uid} AND friend_uid={friendUid}) OR (uid={friendUid} AND friend_uid={uid})")
                                .DeleteAsync();
    }

    public async Task<int> DeleteFriendReq(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .Where("uid", uid)
                                .Where("friend_uid", friendUid)
                                .DeleteAsync();
    }
}