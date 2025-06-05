using GameServer.Models;
using GameServer.Models.DTO;
using GameServer.Repository.Interface;
using GameServer.Services.Interface;
using ZLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using GameServer.Repository;

namespace GameServer.Services;

public class ItemService : IItemService
{
    readonly ILogger<ItemService> _logger;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;
    private readonly Random _random;

    public ItemService(ILogger<ItemService> logger, IGameDb gameDb, IMasterDb masterDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
        _random = new Random();
    }

    public async Task<(ErrorCode, List<ItemInfo>)> GetItemRandom(Int64 accountUid, int gacha_key)
    {
        try
        {
            // 1. gacha_key에 해당하는 가챠 정보를 가져옴
            var gachaInfo = _masterDb._gachaInfoList
                .FirstOrDefault(g => g.gacha_key == gacha_key);

            if (gachaInfo == null)
            {
                _logger.ZLogError($"[ItemService.GetItemRandom] GachaInfo not found: {gacha_key}");
                return (ErrorCode.GachaReceiveFailException, null);
            }

            // 2. 해당 가챠 키에 대한 아이템 확률 목록을 가져옴 
            var gachaRates = _masterDb._gachaRateInfoList
                .Where(r => r.gacha_key == gacha_key)
                .ToList();

            if (gachaRates.Count == 0)
            {
                _logger.ZLogError($"[ItemService.GetItemRandom] No gacha rates found for key: {gacha_key}");
                return (ErrorCode.GachaReceiveFailException, null);
            }

            // 3. 뽑기 결과를 저장할 리스트
            List<ItemInfo> resultItems = new List<ItemInfo>();

            // 전체 확률의 합 계산
            long totalRate = gachaRates.Sum(r => r.rate);
           
            // 4. gachaInfo.count 만큼 반복하여 아이템을 뽑음
            for (int i = 0; i < gachaInfo.count; i++)
            {
                // 랜덤 값 생성 (1 ~ totalRate)
                long randomValue = _random.NextInt64(1, totalRate + 1);

                // 누적 확률로 아이템 선택
                long accumulatedRate = 0;
                int selectedItemId = 0;

                foreach (var rate in gachaRates)
                {
                    accumulatedRate += rate.rate;
                    if (randomValue <= accumulatedRate)
                    {
                        selectedItemId = rate.item_id;
                        break;
                    }
                }

                if (selectedItemId == null)
                {
                    _logger.ZLogError($"[ItemService.GetItemRandom] Failed to select item: {randomValue}/{totalRate}");
                    return (ErrorCode.GachaReceiveFailException, null);
                }

                // 5. 선택된 아이템 정보를 가져옴
                var itemInfo = _masterDb._itemInfoList
                    .FirstOrDefault(item => item.item_id == selectedItemId);

                if (itemInfo == null)
                {
                    _logger.ZLogError($"[ItemService.GetItemRandom] Item info not found: {selectedItemId}");
                    return (ErrorCode.GachaReceiveFailException, null);
                }

                // 6. 결과 리스트에 추가
                resultItems.Add(new ItemInfo
                {
                    item_id = selectedItemId,
                    item_cnt = 1
                });
            }

            IDbTransaction transaction = _gameDb.GetDbConnection().BeginTransaction();
            try
            {
                foreach (var item in resultItems)
                {
                    if (await _gameDb.AddItemInfo(accountUid, item.item_id, item.item_cnt, transaction) != 1) 
                    {
                        transaction.Rollback();
                    }
                }

                transaction.Commit();
            }
            catch 
            {
                transaction.Rollback();
                return (ErrorCode.ItemRegistFail, null);    
            }

            return (ErrorCode.None, resultItems);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[ItemService.GetItemRandom] Exception: {e.Message}, Uid: {accountUid}, GachaKey: {gacha_key}");
            return (ErrorCode.GachaReceiveFailException, null);
        }
    }
    public async Task<(ErrorCode, List<ItemInfo>)> GetItemInfoList(Int64 accountUid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetItemInfoList(accountUid));
        }
        catch (Exception ex)
        {
            // Log the exception
            return (ErrorCode.ItemLoadFail, null);
        }
    }
    public async Task<(ErrorCode, List<Deck>)> GetDeckInfoList(Int64 accountUid)
    {
        try
        {
            List<Deck> deckList = new List<Deck>();

            var result = await _gameDb.GetDeckInfoList(accountUid);
            if (result == null)
            {
                return (ErrorCode.None, null);
            }

            foreach (var deckinfo in result)
            {
                deckList.Add(new Deck(deckinfo.deck_id, deckinfo.deck_list));
            }

            return (ErrorCode.None, deckList);
        }
        catch (Exception ex)
        {
            // Log the exception
            return (ErrorCode.DeckLoadFail, null);
        }
    }

    public async Task<ErrorCode> SaveDeck(Int64 accountUid, int deckId, string deckList)
    {
        try
        {
            _logger.ZLogInformation($"[ItemService.SaveDeck] accountUid: {accountUid}, DeckId: {deckId}");

            // 덱 정보 형식 검증 (예시: JSON 형식인지 확인)
            try
            {
                // 간단한 검증 로직 - 필요에 따라 수정
                if (string.IsNullOrEmpty(deckList))
                {
                    return ErrorCode.DeckInvalidFormat;
                }
            }
            catch
            {
                _logger.ZLogError($"[ItemService.SaveDeck] Invalid deck format: {deckList}");
                return ErrorCode.DeckInvalidFormat;
            }

            // 기존 덱이 있는지 확인
            var existingDeck = await _gameDb.GetDeckInfo(accountUid, deckId);
            int result;

            if (existingDeck == null)
            {
                // 새 덱 생성
                IDbTransaction transaction = _gameDb.GetDbConnection().BeginTransaction();
                try
                {
                    result = await _gameDb.InsertDeck(accountUid, deckId, deckList, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.ZLogError(ex, $"[ItemService.SaveDeck] Insert failed: {ex.Message}");
                    return ErrorCode.DeckSaveFail;
                }
            }
            else
            {
                // 기존 덱 업데이트
                result = await _gameDb.UpdateDeck(accountUid, deckId, deckList);
            }

            return result > 0 ? ErrorCode.None : ErrorCode.DeckSaveFail;
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"[ItemService.SaveDeck] Exception: {ex.Message}, accountUid: {accountUid}, DeckId: {deckId}");
            return ErrorCode.DeckSaveFail;
        }
    }
    public async Task<ErrorCode> SetMainDeck(Int64 accountUid, int deckId)
    {
        try
        {
            _logger.ZLogInformation($"[ItemService.SetMainDeck] accountUid: {accountUid}, DeckId: {deckId}");

            // 해당 덱이 존재하는지 확인
            var deck = await _gameDb.GetDeckInfo(accountUid, deckId);
            if (deck == null)
            {
                _logger.ZLogError($"[ItemService.SetMainDeck] Deck not found: accountUid: {accountUid}, DeckId: {deckId}");
                return ErrorCode.DeckNotFound;
            }

            // 메인 덱 업데이트
            int result = await _gameDb.UpdateMainDeck(accountUid, deckId);
            return result > 0 ? ErrorCode.None : ErrorCode.DeckSetMainFail;
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"[ItemService.SetMainDeck] Exception: {ex.Message}, accountUid: {accountUid}, DeckId: {deckId}");
            return ErrorCode.DeckSetMainFail;
        }
    }
    public async Task<List<int>> GetRandomMainDeckItemList(long accountUid, int count)
    {
        var itemList = await _gameDb.GetDeckItemIdList(accountUid);
        if (itemList.Count == 0) return new List<int>();

        var random = new Random();
        return itemList.OrderBy(x => random.Next()).Take(Math.Min(count, itemList.Count)).ToList();
    }
}