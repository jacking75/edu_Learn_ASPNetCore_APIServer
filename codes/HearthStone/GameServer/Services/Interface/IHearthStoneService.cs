using Microsoft.AspNetCore.Mvc;
using GameServer.Models.DTO;
using GameServer.Models;

namespace GameServer.Services.Interface;

public interface IHearthStoneService
{
    // init 
    public Task<(ErrorCode, Dictionary<int, CardInfo>?)> InitGame(Int64 accountUid, Guid matchGUID);
    //public Task<(ErrorCode, Dictionary<int, CardInfo>?)> ReplaceInitialCards(Int64 accountUid, Guid matchGUID, List<int> replaceIdList);


    // End
    public Task<(ErrorCode, HSGameState, HSPlayerState, HSOpponentState)> GetGameState(Int64 accountUid, Guid matchGUID);
    public Task<ErrorCode> FinishGame(Int64 accountUid, Guid matchGUID, Int64? winnerUid);

    // InPlay
    public Task<(ErrorCode, CardInfo?)> DrawCard(Int64 accountUid, Guid matchGUID);
    public Task<(ErrorCode, bool)> PlayCard(Int64 accountUid, Guid matchGUID, int cardId);
    public Task<(ErrorCode, bool, int)> Attack(Int64 accountUid, Guid matchGUID, int attackerCardId, int targetCardId);
    public Task<(ErrorCode, Int64)> EndTurn(Int64 accountUid, Guid matchGUID);
}