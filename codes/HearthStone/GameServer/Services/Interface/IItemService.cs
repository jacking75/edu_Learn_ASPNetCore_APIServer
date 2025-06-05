using GameServer.Models;

namespace GameServer.Services.Interface;

public interface IItemService
{
    public Task<(ErrorCode, List<ItemInfo>)> GetItemRandom(Int64 accountUid, int gacha_key);

    public Task<(ErrorCode, List<ItemInfo>)> GetItemInfoList(Int64 accountUid);
    public Task<(ErrorCode, List<Deck>)> GetDeckInfoList(Int64 accountUid);
    public Task<ErrorCode> SaveDeck(Int64 accountUid, int deckId, string deckList);
    public Task<ErrorCode> SetMainDeck(Int64 accountUid, int deckId);

    public Task<List<int>> GetRandomMainDeckItemList(long accountUid, int count);
}
