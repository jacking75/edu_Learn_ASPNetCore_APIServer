using GameServer.Services.Interface;
using GameServer.Models.DTO;
using GameServer.Models;
using GameServer.Repository.Interface;
using Microsoft.Extensions.Logging;
using GameServer.Repository;
using System.Linq;

namespace GameServer.Services;

public class HearthStoneService : IHearthStoneService
{
    private readonly ILogger<HearthStoneService> _logger;
    private readonly IMemoryDb _memoryDb;
    private readonly IGameDb _gameDb;
    private readonly IItemService _itemService;
    private readonly IMasterDb _masterDb;

    public HearthStoneService(ILogger<HearthStoneService> logger, IMemoryDb memoryDb, IGameDb gameDb,
        IItemService itemService, IMasterDb masterDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
        _gameDb = gameDb;
        _itemService = itemService;
        _masterDb = masterDb;
    }

    public async Task<(ErrorCode, Dictionary<int, CardInfo>?)> InitGame(Int64 accountUid, Guid matchGUID)
    {
        try
        {
            var initResult = await InitGameValidation(accountUid, matchGUID);
            if (initResult.errorCode != ErrorCode.None)
                return (initResult.errorCode, null);

            var gameInfo = initResult.gameInfo;
            var playerState = initResult.playerState;

            List<int> initCardList = await _itemService.GetRandomMainDeckItemList(accountUid, Global.INIT_CARD_COUNT);
            if (initCardList?.Count != Global.INIT_CARD_COUNT)
            {
                _logger.LogError($"Failed to get initial cards for account: {accountUid}");
                return (ErrorCode.CardInitFail, null);
            }

            playerState = InitPlayerState(playerState, accountUid, initCardList);

            if (!await _memoryDb.UpdatePlayerState(matchGUID, accountUid, playerState))
            {
                _logger.LogError($"Failed to update player state for account: {accountUid}");
                return (ErrorCode.CardInitFail, null);
            }

            return (ErrorCode.None, playerState.HandCardList);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error initializing game: {e.Message}", e);
            return (ErrorCode.CardInitFail, null);
        }
    }

    private async Task<(ErrorCode errorCode, HSGameInfo gameInfo, HSPlayerState playerState)> InitGameValidation(Int64 accountUid, Guid matchGUID)
    {
        var gameInfoTask = _memoryDb.GetMatchInfo(matchGUID);
        var playerStateTask = _memoryDb.GetPlayerState(matchGUID, accountUid);

        await Task.WhenAll(gameInfoTask, playerStateTask);
        var gameInfo = gameInfoTask.Result;
        var playerState = playerStateTask.Result;

        if (gameInfo == null)
        {
            _logger.LogError($"Game info not found for match GUID: {matchGUID}");
            return (ErrorCode.CardInitFail, null, null);
        }

        return (ErrorCode.None, gameInfo, playerState);
    }

    private HSPlayerState InitPlayerState(HSPlayerState playerState, Int64 accountUid, List<int> initCardList)
    {
        if (playerState == null)
        {
            playerState = new HSPlayerState
            {
                AccountUid = accountUid,
                DeckCount = Global.INITIAL_DECK_COUNT,
                HandCardList = new Dictionary<int, CardInfo>(),
                FieldCardList = new Dictionary<int, CardInfo>(),
                AttackCardList = new List<int>(),
                UseCardList = new List<int>(),
                HasDrawnCardThisTurn = false
            };
        }

        foreach(var cardId in initCardList)
        {
            var itemdetailinfo = _masterDb._itemDetailInfoList[cardId];
            playerState.HandCardList.Add(cardId, new CardInfo 
            {
                ItemId = itemdetailinfo.CardInfo.ItemId,
                Attack = itemdetailinfo.CardInfo.Attack,
                ManaCost = itemdetailinfo.CardInfo.ManaCost,
                Hp = itemdetailinfo.CardInfo.Hp
            });
            playerState.UseCardList.Add(cardId);
        }

        return playerState;
    }

    //public async Task<(ErrorCode, Dictionary<int, CardInfo>?)> ReplaceInitialCards(Int64 accountUid, Guid matchGUID, List<int> replaceIdList)
    //{
    //    try
    //    {
    //        if (replaceIdList == null || replaceIdList.Count == 0)
    //        {
    //            return (ErrorCode.None, null);
    //        }

    //        var playerState = await _memoryDb.GetPlayerState(matchGUID, accountUid);
    //        var validationResult = ValidateReplaceCardIndices(playerState, accountUid, matchGUID, replaceIdList);
    //        if (validationResult.errorCode != ErrorCode.None)
    //            return (validationResult.errorCode, null);

    //        var newCards = await _itemService.GetRandomMainDeckItemList(accountUid, replaceIdList.Count);
    //        if (newCards == null || newCards.Count != replaceIdList.Count)
    //        {
    //            _logger.LogError($"Failed to get replacement cards for account: {accountUid}");
    //            return (ErrorCode.CardReplaceFail, null);
    //        }

    //        Dictionary<int,CardInfo> resultCards = ReplaceCards(playerState.HandCardList, replaceIdList, newCards);

    //        playerState.HandCardList = resultCards;
    //        await _memoryDb.UpdatePlayerState(matchGUID, accountUid, playerState);

    //        return (ErrorCode.None, resultCards);
    //    }
    //    catch (Exception e)
    //    {
    //        _logger.LogError($"Error replacing initial cards: {e.Message}", e);
    //        return (ErrorCode.CardReplaceFail, null);
    //    }
    //}

    private (ErrorCode errorCode, HSPlayerState playerState) ValidateReplaceCardIndices(HSPlayerState playerState, Int64 accountUid, Guid matchGUID, List<int> replaceIdList)
    {
        if (playerState == null || playerState.HandCardList == null || playerState.HandCardList.Count == 0)
        {
            _logger.LogError($"Player state or hand cards not found: {accountUid}, match: {matchGUID}");
            return (ErrorCode.CardReplaceFail, null);
        }

        foreach (var id in replaceIdList)
        { 
           if(playerState.HandCardList.ContainsKey(id))
           {
                _logger.LogError($"Invalid replace index for account: {accountUid}");
                return (ErrorCode.CardReplaceFail, null);
           }
        }

        return (ErrorCode.None, playerState);
    }

    //private Dictionary<int, CardInfo> ReplaceCards(Dictionary<int, CardInfo> originalCards, List<int> replaceitemid, List<int> newCards)
    //{
    //    var resultCards = new Dictionary<int, CardInfo>(originalCards);
    //    foreach(var id in replaceitemid)
    //    {
    //        var newcard = _masterDb._itemDetailInfoList[newCards.];
    //        resultCards.Remove(id);
    //        resultCards.Add(,new CardInfo { 
    //            ItemId = newcard.CardInfo.ItemId, 
    //            Attack = newcard.CardInfo.Attack, 
    //            Hp = newcard.CardInfo.Hp,
    //            ManaCost = newcard.CardInfo.ManaCost
    //        });
    //    }
    //    return resultCards;
    //}

    public (bool, Int64?) IsGameOver(HSGameInfo gameInfo)
    {
        if (gameInfo.IsGameOver)
        {
            return (true, gameInfo.WinnerUid);
        }

        foreach (var player in gameInfo.GameUserList)
        {
            if (player?.Hp <= 0)
            {
                var winner = gameInfo.GameUserList.FirstOrDefault(p => p.AccountUid != player.AccountUid);
                return (true, winner?.AccountUid);
            }
        }

        return (false, null);
    }

    public async Task<(ErrorCode, HSGameState, HSPlayerState, HSOpponentState)> GetGameState(Int64 accountUid, Guid matchGUID)
    {
        try
        {
            var gameInfo = await _memoryDb.GetMatchInfo(matchGUID);
            if (gameInfo == null)
            {
                _logger.LogError($"Game info not found for match GUID: {matchGUID}");
                return (ErrorCode.MatchStatusCheckFailException, null, null, null);
            }

            var (isGameOver, winnerUid) = IsGameOver(gameInfo);
            if (isGameOver && !gameInfo.IsGameOver)
            {
                gameInfo = await SetMatchComplete(gameInfo, winnerUid);
            }

            await CheckAndHandleTurnTimeout(gameInfo, matchGUID);

            var playerState = await _memoryDb.GetPlayerState(matchGUID, accountUid);
            var opponentState = await _memoryDb.GetPlayerState(matchGUID, gameInfo.GameUserList.FirstOrDefault(info => info.AccountUid != accountUid).AccountUid);

            return (ErrorCode.None, gameInfo.ToGameState(), playerState, opponentState?.ToHSOppentState());
        }
        catch (Exception e)
        {
            _logger.LogError($"Error getting game state: {e.Message}", e);
            return (ErrorCode.MatchStatusCheckFailException, null, null, null);
        }
    }

    private async Task<HSGameInfo> CheckAndHandleTurnTimeout(HSGameInfo gameInfo, Guid matchGUID)
    {
        if (!gameInfo.IsGameOver && gameInfo.LastActionTime.AddSeconds(Global.MATCH_TIMEOUT) < DateTime.UtcNow)
        {
            var currentTurnUid = gameInfo.CurrentTurnUid;
            var turnResult = await EndTurn(currentTurnUid, matchGUID);

            if (turnResult.Item1 == ErrorCode.None)
            {
                gameInfo = await _memoryDb.GetMatchInfo(matchGUID);
            }
        }

        return gameInfo;
    }

    public async Task<ErrorCode> FinishGame(Int64 accountUid, Guid matchGUID, Int64? winnerUid = null)
    {
        try
        {
            var gameInfo = await _memoryDb.GetMatchInfo(matchGUID);
            if (gameInfo == null)
            {
                _logger.LogError($"Game info not found for match GUID: {matchGUID}");
                return ErrorCode.MatchStatusCheckFailException;
            }

            if (gameInfo.IsGameOver)
            {
                _logger.LogWarning($"Game is already over: {matchGUID}, winner: {gameInfo.WinnerUid}");
                return ErrorCode.None;
            }

            var (isOver, systemWinnerUid) = IsGameOver(gameInfo);

            if (systemWinnerUid.HasValue && systemWinnerUid.Value != winnerUid)
            {
                winnerUid = systemWinnerUid;
            }

            await SetMatchComplete(gameInfo, winnerUid);
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error finishing game: {e.Message}", e);
            return ErrorCode.MatchStatusCheckFailException;
        }
    }

    private async Task<HSGameInfo> SetMatchComplete(HSGameInfo gameInfo, Int64? winnerUid)
    {
        if (gameInfo == null)
            return null;

        gameInfo.IsGameOver = true;
        gameInfo.WinnerUid = winnerUid;

        await _memoryDb.MarkMatchCompleted(gameInfo);

        return gameInfo;
    }

    public async Task<(ErrorCode, CardInfo?)> DrawCard(Int64 accountUid, Guid matchGUID)
    {
        try
        {
            var validationResult = await ValidateDrawCardPreconditions(accountUid, matchGUID);
            if (validationResult.errorCode != ErrorCode.None)
                return (validationResult.errorCode, null);

            var playerState = validationResult.playerState;
            
            var drawnCards = await _itemService.GetRandomMainDeckItemList(accountUid, 1);
            if (drawnCards == null || drawnCards.Count == 0)
            {
                _logger.LogError($"Failed to draw card for player: {accountUid}");
                return (ErrorCode.CardDrawFail, null);
            }

            var drawnCard = drawnCards[0];
            var cardInfo = await UpdatePlayerStateAfterDraw(matchGUID, accountUid, playerState, drawnCard);

            return (ErrorCode.None, cardInfo);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error drawing card: {e.Message}", e);
            return (ErrorCode.CardDrawFail, null);
        }
    }

    private async Task<(ErrorCode errorCode, HSGameInfo gameInfo, HSPlayerState playerState)> ValidateDrawCardPreconditions(Int64 accountUid, Guid matchGUID)
    {
        var gameInfoTask = _memoryDb.GetMatchInfo(matchGUID);
        var playerStateTask = _memoryDb.GetPlayerState(matchGUID, accountUid);

        await Task.WhenAll(gameInfoTask, playerStateTask);
        var gameInfo = gameInfoTask.Result;
        var playerState = playerStateTask.Result;

        if (gameInfo == null)
        {
            _logger.LogError($"Game info not found for match GUID: {matchGUID}");
            return (ErrorCode.MatchStatusCheckFailException, null, null);
        }

        if (gameInfo.CurrentTurnUid != accountUid)
        {
            _logger.LogError($"Not player's turn. AccountUid: {accountUid}, CurrentTurnUid: {gameInfo.CurrentTurnUid}");
            return (ErrorCode.NotPlayersTurn, null, null);
        }

        if (playerState == null)
        {
            _logger.LogError($"Player state not found for account: {accountUid}");
            return (ErrorCode.PlayerStateNotFound, null, null);
        }

        if (playerState.HasDrawnCardThisTurn)
        {
            _logger.LogError($"Player has already drawn a card this turn: {accountUid}");
            return (ErrorCode.CardDrawLimitReached, null, null);
        }

        if (playerState.DeckCount <= 0)
        {
            _logger.LogError($"No cards left in deck for player: {accountUid}");
            return (ErrorCode.NoCardsInDeck, null, null);
        }

        if (playerState.HandCardList.Count >= Global.MAX_HAND_CARDS)
        {
            _logger.LogError($"Hand cards full for player: {accountUid}");
            return (ErrorCode.HandCardsFull, null, null);
        }

        return (ErrorCode.None, gameInfo, playerState);
    }
    private async Task<CardInfo> UpdatePlayerStateAfterDraw(Guid matchGUID, Int64 accountUid, HSPlayerState playerState, int drawnCard)
    {
        var itemdetailinfo = _masterDb._itemDetailInfoList[drawnCard];
        var cardInfo = new CardInfo
        {
            ItemId = itemdetailinfo.CardInfo.ItemId,
            Attack = itemdetailinfo.CardInfo.Attack,
            ManaCost = itemdetailinfo.CardInfo.ManaCost,
            Hp = itemdetailinfo.CardInfo.Hp
        };

        playerState.HandCardList.Add(drawnCard, cardInfo);

        playerState.DeckCount--;
        playerState.HasDrawnCardThisTurn = true;
        playerState.UseCardList.Add(drawnCard);

        await _memoryDb.UpdatePlayerState(matchGUID, accountUid, playerState);
        return cardInfo;
    }
    public async Task<(ErrorCode, bool)> PlayCard(Int64 accountUid, Guid matchGUID, int cardId)
    {
        try
        {
            var validationResult = await ValidatePlayCardPreconditions(accountUid, matchGUID, cardId);
            if (validationResult.errorCode != ErrorCode.None)
                return (validationResult.errorCode, false);

            var gameInfo = validationResult.gameInfo;
            var playerState = validationResult.playerState;
            var player = validationResult.player;

            int manaCost = GetCardManaCost(cardId);

            if (player.Mana < manaCost)
            {
                _logger.LogError($"Not enough mana. Required: {manaCost}, Available: {player.Mana}");
                return (ErrorCode.NotEnoughMana, false);
            }

            playerState.HandCardList.Remove(cardId);

            // CardInfo 생성하여 필드에 추가
            var cardInfo = _masterDb._itemDetailInfoList.TryGetValue(cardId, out var itemDetail)
                ? itemDetail.CardInfo
                : new CardInfo { ItemId = cardId };

            playerState.FieldCardList[cardId] = cardInfo;
            player.Mana -= manaCost;

            var updateResult = await UpdateGameStateAfterPlayCard(matchGUID, accountUid, gameInfo, playerState);
            if (!updateResult)
            {
                return (ErrorCode.CardPlayFail, false);
            }

            return (ErrorCode.None, true);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error playing card: {e.Message}", e);
            return (ErrorCode.CardPlayFail, false);
        }
    }

    private async Task<(ErrorCode errorCode, HSGameInfo gameInfo, HSPlayerState playerState, HSGameUserInfo player)>
        ValidatePlayCardPreconditions(Int64 accountUid, Guid matchGUID, int cardId)
    {
        var gameInfoTask = _memoryDb.GetMatchInfo(matchGUID);
        var playerStateTask = _memoryDb.GetPlayerState(matchGUID, accountUid);

        await Task.WhenAll(gameInfoTask, playerStateTask);
        var gameInfo = gameInfoTask.Result;
        var playerState = playerStateTask.Result;

        if (gameInfo == null)
        {
            _logger.LogError($"Game info not found for match GUID: {matchGUID}");
            return (ErrorCode.MatchStatusCheckFailException, null, null, null);
        }

        if (gameInfo.CurrentTurnUid != accountUid)
        {
            _logger.LogError($"Not player's turn. AccountUid: {accountUid}, CurrentTurnUid: {gameInfo.CurrentTurnUid}");
            return (ErrorCode.NotPlayersTurn, null, null, null);
        }

        if (playerState == null)
        {
            _logger.LogError($"Player state not found for account: {accountUid}");
            return (ErrorCode.PlayerStateNotFound, null, null, null);
        }

        if (playerState.FieldCardList.Count >= Global.MAX_FIELD_CARDS)
        {
            _logger.LogError($"Field cards full for player: {accountUid}");
            return (ErrorCode.FieldCardsFull, null, null, null);
        }

        // Dictionary를 사용하여 카드 ID로 직접 확인
        if (!playerState.HandCardList.ContainsKey(cardId))
        {
            _logger.LogError($"Card {cardId} not found in player's hand. AccountUid: {accountUid}");
            return (ErrorCode.CardNotInHand, null, null, null);
        }

        var player = gameInfo.GameUserList.FirstOrDefault(u => u.AccountUid == accountUid);
        if (player == null)
        {
            _logger.LogError($"Player not found in game: {accountUid}");
            return (ErrorCode.PlayerNotFound, null, null, null);
        }

        return (ErrorCode.None, gameInfo, playerState, player);
    }

    private int GetCardManaCost(int cardId)
    {
        if (_masterDb._itemDetailInfoList.TryGetValue(cardId, out var itemDetail))
        {
            return itemDetail.CardInfo?.ManaCost ?? cardId;
        }
        return cardId;
    }

    private async Task<bool> UpdateGameStateAfterPlayCard(Guid matchGUID, Int64 accountUid, HSGameInfo gameInfo, HSPlayerState playerState)
    {
        var updatePlayerTask = _memoryDb.UpdatePlayerState(matchGUID, accountUid, playerState);
        var updateGameTask = _memoryDb.UpdateGameInfo(gameInfo);

        await Task.WhenAll(updatePlayerTask, updateGameTask);

        if (!updatePlayerTask.Result || !updateGameTask.Result)
        {
            _logger.LogError($"Failed to update game state after playing card: {accountUid}");
            return false;
        }

        return true;
    }

    public async Task<(ErrorCode, bool, int)> Attack(Int64 accountUid, Guid matchGUID, int attackerCardId, int targetCardId)
    {
        try
        {
            var validationResult = await ValidateAttackPreconditions(accountUid, matchGUID, attackerCardId);
            if (validationResult.errorCode != ErrorCode.None)
            {
                return (validationResult.errorCode, false, 0);
            }

            var gameInfo = validationResult.gameInfo;
            var playerState = validationResult.playerState; // playerState를 저장
            var player = validationResult.player;
            var opponent = validationResult.opponent;

            var attackValues = CalculateAttackValues(attackerCardId);
            if (player.Mana < attackValues.manaCost)
            {
                _logger.LogError($"Not enough mana for attack. Required: {attackValues.manaCost}, Available: {player.Mana}");
                return (ErrorCode.NotEnoughMana, false, 0);
            }

            // 공격한 카드를 AttackCardList에 추가 (중복 방지를 위한 검사 포함)
            if (!playerState.AttackCardList.Contains(attackerCardId))
            {
                playerState.AttackCardList.Add(attackerCardId);
                // 플레이어 상태 업데이트
                await _memoryDb.UpdatePlayerState(matchGUID, accountUid, playerState);
            }

            player.Mana -= attackValues.manaCost;

            var attackResult = await ApplyAttackDamage(
                gameInfo, matchGUID, accountUid, opponent,
                targetCardId, attackValues.damage);

            if (attackResult.errorCode != ErrorCode.None)
            {
                return (attackResult.errorCode, false, 0);
            }

            await _memoryDb.UpdateGameInfo(gameInfo);

            return (ErrorCode.None, true, attackValues.damage);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error attacking: {e.Message}", e);
            return (ErrorCode.AttackFail, false, 0);
        }
    }

    private async Task<(ErrorCode errorCode, HSGameInfo gameInfo, HSPlayerState playerState, HSGameUserInfo player, HSGameUserInfo opponent)> ValidateAttackPreconditions(Int64 accountUid, Guid matchGUID, int attackerCardId)
    {
        var gameInfo = await _memoryDb.GetMatchInfo(matchGUID);
        if (gameInfo == null)
        {
            _logger.LogError($"Game info not found for match GUID: {matchGUID}");
            return (ErrorCode.MatchStatusCheckFailException, null, null, null, null);
        }

        if (gameInfo.CurrentTurnUid != accountUid)
        {
            _logger.LogError($"Not player's turn. AccountUid: {accountUid}, CurrentTurnUid: {gameInfo.CurrentTurnUid}");
            return (ErrorCode.NotPlayersTurn, null, null, null, null);
        }

        var playerState = await _memoryDb.GetPlayerState(matchGUID, accountUid);
        if (playerState == null || !playerState.FieldCardList.ContainsKey(attackerCardId))
        {
            _logger.LogError($"Attacker card not found on player's field. AccountUid: {accountUid}, CardId: {attackerCardId}");
            return (ErrorCode.CardNotOnField, null, null, null, null);
        }

        // 이미 공격한 카드인지 확인
        if (playerState.AttackCardList != null && playerState.AttackCardList.Contains(attackerCardId))
        {
            _logger.LogError($"Card {attackerCardId} has already attacked this turn. AccountUid: {accountUid}");
            return (ErrorCode.CardAlreadyAttacked, null, null, null, null); // ErrorCode 추가 필요
        }

        var player = gameInfo.GameUserList.FirstOrDefault(u => u.AccountUid == accountUid);
        if (player == null)
        {
            _logger.LogError($"Player not found in game: {accountUid}");
            return (ErrorCode.PlayerNotFound, null, null, null, null);
        }

        var opponent = gameInfo.GameUserList.FirstOrDefault(u => u.AccountUid != accountUid);
        if (opponent == null)
        {
            _logger.LogError($"Opponent not found for account: {accountUid}");
            return (ErrorCode.OpponentNotFound, null, null, null, null);
        }

        return (ErrorCode.None, gameInfo, playerState, player, opponent);
    }

    private (int manaCost, int damage) CalculateAttackValues(int attackerCardId)
    {
        int attackManaCost = 0;
        int damage = attackerCardId;

        if (_masterDb._itemDetailInfoList.TryGetValue(attackerCardId, out var itemDetail))
        {
            var cardInfo = itemDetail.CardInfo;
            if (cardInfo != null)
            {
                attackManaCost = cardInfo.ManaCost;
                damage = cardInfo.Attack;
            }
        }

        return (attackManaCost, damage);
    }

    private async Task<(ErrorCode errorCode, bool success, int damage)> ApplyAttackDamage(
        HSGameInfo gameInfo, Guid matchGUID, Int64 attackerUid,
        HSGameUserInfo opponent, int targetCardId, int damage)
    {
        if (targetCardId == -1)
        {
            return await ApplyPlayerAttack(gameInfo, attackerUid, opponent, damage);
        }
        else
        {
            return await ApplyCardAttack(matchGUID, opponent.AccountUid, targetCardId, damage);
        }
    }

    private async Task<(ErrorCode errorCode, bool success, int damage)> ApplyPlayerAttack(
        HSGameInfo gameInfo, Int64 attackerUid, HSGameUserInfo opponent, int damage)
    {
        opponent.Hp -= damage;
        _logger.LogInformation($"Player attack: dealing {damage} damage to opponent. Remaining HP: {opponent.Hp}");

        if (opponent.Hp <= 0)
        {
            await SetMatchComplete(gameInfo, attackerUid);
        }

        return (ErrorCode.None, true, damage);
    }

    private async Task<(ErrorCode errorCode, bool success, int damage)> ApplyCardAttack(Guid matchGUID, Int64 opponentUid, int targetCardId, int damage)
    {
        var opponentState = await _memoryDb.GetPlayerState(matchGUID, opponentUid);

        if (opponentState == null || !opponentState.FieldCardList.ContainsKey(targetCardId))
        {
            _logger.LogError($"Target card not found on opponent's field. OpponentUid: {opponentUid}, CardId: {targetCardId}");
            return (ErrorCode.TargetCardNotFound, false, 0);
        }

        _logger.LogInformation($"Card attack: destroying opponent card {targetCardId}");
        opponentState.FieldCardList.TryGetValue(targetCardId, out var cardInfo);
        cardInfo.Hp = int.Max(cardInfo.Hp - damage, 0);
        
        if(cardInfo.Hp <= 0)
            opponentState.FieldCardList.Remove(targetCardId);
        
        await _memoryDb.UpdatePlayerState(matchGUID, opponentUid, opponentState);

        return (ErrorCode.None, true, 0);
    }

    public async Task<(ErrorCode, Int64)> EndTurn(Int64 accountUid, Guid matchGUID)
    {
        try
        {
            var validationResult = await ValidateEndTurnPreconditions(accountUid, matchGUID);
            if (validationResult.errorCode != ErrorCode.None)
            {
                return (validationResult.errorCode, 0);
            }

            var gameInfo = validationResult.gameInfo;
            var nextPlayer = validationResult.nextPlayer;

            // 현재 플레이어의 AttackCardList 초기화
            var currentPlayerState = await _memoryDb.GetPlayerState(matchGUID, accountUid);
            if (currentPlayerState != null)
            {
                // AttackCardList 초기화
                currentPlayerState.AttackCardList.Clear();
                await _memoryDb.UpdatePlayerState(matchGUID, accountUid, currentPlayerState);
            }

            gameInfo.CurrentTurnUid = nextPlayer.AccountUid;
            gameInfo.TurnCount++;
            gameInfo.LastActionTime = DateTime.UtcNow;

            if (gameInfo.TurnCount >= Global.MAX_TURN_COUNT)
            {
                await SetMatchComplete(gameInfo, null);
                return (ErrorCode.None, nextPlayer.AccountUid);
            }

            await UpdateTurnManaAndState(gameInfo, matchGUID, nextPlayer);

            return (ErrorCode.None, nextPlayer.AccountUid);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error ending turn: {e.Message}", e);
            return (ErrorCode.EndTurnFail, 0);
        }
    }

    private async Task<(ErrorCode errorCode, HSGameInfo gameInfo, HSGameUserInfo nextPlayer)> ValidateEndTurnPreconditions(Int64 accountUid, Guid matchGUID)
    {
        var gameInfo = await _memoryDb.GetMatchInfo(matchGUID);
        if (gameInfo == null)
        {
            _logger.LogError($"Game info not found for match GUID: {matchGUID}");
            return (ErrorCode.MatchStatusCheckFailException, null, null);
        }

        if (gameInfo.CurrentTurnUid != accountUid &&
            gameInfo.LastActionTime.AddSeconds(Global.MATCH_TIMEOUT) >= DateTime.UtcNow)
        {
            _logger.LogError($"Not player's turn. AccountUid: {accountUid}, CurrentTurnUid: {gameInfo.CurrentTurnUid}");
            return (ErrorCode.NotPlayersTurn, null, null);
        }

        var nextPlayer = gameInfo.GameUserList.FirstOrDefault(u => u.AccountUid != accountUid);
        if (nextPlayer == null)
        {
            _logger.LogError($"Next player not found for account: {accountUid}");
            return (ErrorCode.OpponentNotFound, null, null);
        }

        return (ErrorCode.None, gameInfo, nextPlayer);
    }

    private async Task UpdateTurnManaAndState(HSGameInfo gameInfo, Guid matchGUID, HSGameUserInfo nextPlayer)
    {
        if (gameInfo.TurnCount % 2 == 0)
        {
            foreach (var user in gameInfo.GameUserList)
            {
                user.Mana = Math.Min((Global.MAX_TURN_COUNT / 2), (gameInfo.TurnCount/2) + 1);
            }
        }

        var nextPlayerState = await _memoryDb.GetPlayerState(matchGUID, nextPlayer.AccountUid);
        if (nextPlayerState != null)
        {
            nextPlayerState.HasDrawnCardThisTurn = false;
            await _memoryDb.UpdatePlayerState(matchGUID, nextPlayer.AccountUid, nextPlayerState);
        }

        await _memoryDb.UpdateGameInfo(gameInfo);
    }
}
