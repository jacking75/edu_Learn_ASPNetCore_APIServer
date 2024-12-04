using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;

namespace APIServer.Services;

public interface IMemoryDb : IAsyncDisposable
{

    public void Init(string address);


    public string GetGameNotice();


    public Task<bool> LoadGameNotice(string key);


    public Task<ErrorCode> CheckCertifiedUser(string email, string authToken, Int32 appVersion, Int32 masterDataVersion);



    #region Set Services

    public Task<ErrorCode> RegistCertifiedUserInfo(string id, CertifiedUser value);


    public Task<ErrorCode> UpdateCertifiedUserInfo(string id, CertifiedUser value);


    public Task<ErrorCode> RegistUserBattleInfo(string id, UserBattleInfo value);


    public Task<ErrorCode> UpdateUserBattleInfo(string id, UserBattleInfo value);


    public Task<bool> TryLockUserRequest(string id);


    public Task<bool> AddChannelChatInfo(Int32 channelNumber, ChatInfo value);

    #endregion



    #region Get Services


    public Task<(bool, CertifiedUser)> GetCertifiedUser(string id);


    public Task<(bool, UserBattleInfo)> GetUserBattleInfo(string id);


    public Task<(bool, List<ChatInfo>)> GetChannelChatInfoList(Int32 channelNumber, Int32 count, string messageId = null);

    #endregion



    #region Del Services

    public Task<bool> UnlockUserRequest(string id);


    public Task<bool> RemoveUserBattleInfo(string id);

    #endregion
}
