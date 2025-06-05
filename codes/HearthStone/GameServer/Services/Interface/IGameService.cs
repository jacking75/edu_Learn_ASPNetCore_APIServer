using GameServer.Models;

namespace GameServer.Services.Interface;
public interface IGameService
{
    public Task<ErrorCode> InitNewUserGameData(Int64 accountUid);
    public Task<(ErrorCode, List<AssetInfo>)> GetAssetInfoList(Int64 accountUid);
}

