using GameAPIServer.Servicies.Interfaces;
using GameAPIServer.Models.DAO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZLogger;
using GameAPIServer.Repository.Interfaces;

namespace GameAPIServer.Servicies;

public class FriendService : IFriendService
{
    readonly ILogger<FriendService> _logger;
    readonly IGameDb _gameDb;

    public FriendService(ILogger<FriendService> logger, IGameDb gameDb)
    {
        _gameDb = gameDb;
        _logger = logger;
    }

    public async Task<ErrorCode> SendFriendReq(int uid, int friendUid)
    {
        try
        {
            if (uid == friendUid)
            {
                _logger.ZLogDebug(
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendAcceptFailSameUid}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendAcceptFailSameUid;
            }

            GdbUserInfo userInfo = await _gameDb.GetUserByUid(friendUid);
            //없는 유저일 때
            if (userInfo is null)
            {
                _logger.ZLogDebug(
                $"[Friend.SendFriendReq] ErrorCode: {ErrorCode.FriendSendReqFailUserNotExist}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendSendReqFailUserNotExist;
            }
            //이미 친구신청 했거나 친구일 때
            GdbFriendInfo friendReqInfo = await _gameDb.GetFriendReqInfo(uid, friendUid);
            if (friendReqInfo is not null)
            {
                _logger.ZLogDebug(
                $"[Friend.SendFriendReq] ErrorCode: {ErrorCode.FriendSendReqFailAlreadyExist}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendSendReqFailAlreadyExist;
            }
            //상대의 친구요청이 와있을 때
            friendReqInfo = await _gameDb.GetFriendReqInfo(friendUid, uid);
            if(friendReqInfo is not null)
            {
                _logger.ZLogDebug(
                                   $"[Friend.SendFriendReq] ErrorCode: {ErrorCode.FriendSendReqFailNeedAccept}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendSendReqFailNeedAccept;
            }
            //친구 요청
            var rowCount = await _gameDb.InsertFriendReq(uid, friendUid);
            if (rowCount != 1)
            {
                _logger.ZLogDebug(
                $"[Friend.SendFriendReq] ErrorCode: {ErrorCode.FriendSendReqFailInsert}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendSendReqFailInsert;
            }
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Friend.SendFriendReq] ErrorCode: {ErrorCode.FriendSendReqFailException}, Uid: {uid}, FriendUid : {friendUid}");
            return ErrorCode.FriendSendReqFailException;
        }
    }

    public async Task<ErrorCode> AcceptFriendReq(int uid, int friendUid)
    {
        try
        {
            if (uid == friendUid)
            {
                _logger.ZLogDebug(
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendAcceptFailSameUid}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendAcceptFailSameUid;
            }

            GdbUserInfo userInfo = await _gameDb.GetUserByUid(friendUid);
            //없는 유저일 때
            if (userInfo is null)
            {
                _logger.ZLogDebug(
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendSendReqFailUserNotExist}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.AcceptFriendRequestFailUserNotExist;
            }
            //친구 요청이 안왔거나, 이미 친구 일 때
            GdbFriendInfo friendReqInfo = await _gameDb.GetFriendReqInfo(friendUid, uid);
            if (friendReqInfo is null || friendReqInfo.friend_yn == true)
            {
                _logger.ZLogDebug(
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendSendReqFailAlreadyExist}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.AcceptFriendRequestFailAlreadyFriend;
            }
            //친구 요청 수락
            return await AcceptRequest(uid, friendUid);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.AcceptFriendRequestFailException}, Uid: {uid}, FriendUid : {friendUid}");
            return ErrorCode.AcceptFriendRequestFailException;
        }
    }

    public async Task<(ErrorCode, IEnumerable<GdbFriendInfo>)> GetFriendList(int uid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetFriendInfoList(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[Friend.GetFriendList] ErrorCode: {ErrorCode.FriendGetListFailException}, Uid: {uid}");
            return (ErrorCode.FriendGetListFailException, null);
        }
    }

    public async Task<ErrorCode> DeleteFriend(int uid, int friendUid)
    {
        try
        {
            if (uid == friendUid)
            {
                _logger.ZLogDebug(
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendAcceptFailSameUid}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendAcceptFailSameUid;
            }

            //친구가 아닐 때
            GdbFriendInfo friendInfo = await _gameDb.GetFriendReqInfo(uid, friendUid);
            if (friendInfo is null || friendInfo.friend_yn==false)
            {
                _logger.ZLogDebug(
                $"[Friend.DeleteFriend] ErrorCode: {ErrorCode.FriendDeleteFailNotFriend}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendDeleteFailNotFriend;
            }

            var rowCount = await _gameDb.DeleteFriendEachOther(uid, friendUid);
            if(rowCount != 2)
            {
                _logger.ZLogDebug(
                                   $"[Friend.DeleteFriend] ErrorCode: {ErrorCode.FriendDeleteFailDelete}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendDeleteFailDelete;
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[Friend.DeleteFriend] ErrorCode: {ErrorCode.FriendDeleteFailException}, Uid: {uid}, FriendUid : {friendUid}");
            return ErrorCode.FriendDeleteFailException;
        }
    }

    public async Task<ErrorCode> CancelFriendReq(int uid, int friendUid)
    {
        try
        {
            if (uid == friendUid)
            {
                _logger.ZLogDebug(
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendAcceptFailSameUid}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendAcceptFailSameUid;
            }
            //친구 요청을 안했거나 친구 상태 일때
            GdbFriendInfo friendInfo = await _gameDb.GetFriendReqInfo(uid, friendUid);
            if (friendInfo is null || friendInfo.friend_yn == true)
            {
                _logger.ZLogDebug(
                $"[Friend.DeleteFriendReq] ErrorCode: {ErrorCode.FriendDeleteReqFailNotFriend}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendDeleteReqFailNotFriend;
            }

            var rowCount = await _gameDb.DeleteFriendReq(uid, friendUid);
            if (rowCount != 1)
            {
                _logger.ZLogDebug(
                $"[Friend.DeleteFriendReq] ErrorCode: {ErrorCode.FriendDeleteReqFailDelete}, Uid: {uid}, FriendUid : {friendUid}");
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[Friend.DeleteFriendReq] ErrorCode: {ErrorCode.FriendDeleteReqFailException}, Uid: {uid}, FriendUid : {friendUid}");
            return ErrorCode.FriendDeleteReqFailException;
        }
    }

    async Task<ErrorCode> AcceptRequest(int uid, int friendUid)
    {
        var transaction = _gameDb.GDbConnection().BeginTransaction();
        try
        {
            var rowCount = await _gameDb.InsertFriendReq(uid, friendUid, transaction, true);
            if (rowCount != 1)
            {
                _logger.ZLogDebug(
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendSendReqFailInsert}, Uid: {uid}, FriendUid : {friendUid}");
                transaction.Rollback();
                return ErrorCode.FriendSendReqFailInsert;
            }

            rowCount = await _gameDb.UpdateFriendReqAccept(uid, friendUid, transaction, true);
            if (rowCount != 1)
            {
                _logger.ZLogDebug(
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendSendReqFailInsert}, Uid: {uid}, FriendUid : {friendUid}");
                transaction.Rollback();
                return ErrorCode.FriendSendReqFailInsert;
            }

            transaction.Commit();
            return ErrorCode.None;
        }
        catch(Exception e)
        {
            _logger.ZLogError(e,
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendAcceptFailException}, Uid: {uid}, FriendUid : {friendUid}");
            return ErrorCode.FriendAcceptFailException;
        }
        finally
        {
            transaction.Dispose();
        }
    }
}
