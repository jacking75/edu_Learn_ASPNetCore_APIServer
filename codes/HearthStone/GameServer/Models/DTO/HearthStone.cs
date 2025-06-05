using GameServer.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using GameServer.Models;

namespace GameServer.Models.DTO;

public class HSGameUserInfo
{ 
    public Int64 AccountUid { get; set; }
    public int Hp { get; set; }
    public int Mana { get; set; }
}

public class HSGameInfo
{
    // 기본 식별 정보
    public Guid MatchGUID { get; set; }
    public int MatchType { get; set; } = 0; // 0: pvp, 1: pve

    // 게임 진행 정보
    public DateTime StartTime { get; set; }
    public DateTime LastActionTime { get; set; } = DateTime.UtcNow;
    public Int64 CurrentTurnUid { get; set; }
    public int TurnCount { get; set; } = 0;

    // 플레이어 정보
    public List<HSGameUserInfo> GameUserList { get; set; }

    // 게임 상태 정보
    public bool IsGameOver { get; set; } = false;
    public Int64? WinnerUid { get; set; } = null;

    // 상태 변환 메서드 - 클라이언트에 전송할 HSGameState로 변환
    public HSGameState ToGameState()
    {
        return new HSGameState
        {
            MatchGUID = this.MatchGUID,
            Players = this.GameUserList,
            CurrentTurnUid = this.CurrentTurnUid,
            IsGameOver = this.IsGameOver,
            WinnerUid = this.WinnerUid,
            LastActionTime = this.LastActionTime,
            TurnCount = this.TurnCount
        };
    }
}
public class InitGameRequest
{
    public Guid MatchGUID { get; set; }
}
    public class InitGameResponse : ErrorCodeDTO
{
    public Dictionary<int, CardInfo> InitialCardList { get; set; }
}

public class ReplaceCardsRequest
{
    public Guid MatchGUID { get; set; }
    public Dictionary<int, CardInfo> ReplaceCardIndexList { get; set; } // 교체할 카드 인덱스
}
public class ReplaceCardsResponse : ErrorCodeDTO
{
    public List<int> InitialCardList { get; set; }
}
// GameState 응답을 위한 DTO 추가
public class GameStateRequest 
{
    public Guid MatchGuid { get; set; }
}
public class GameStateResponse : ErrorCodeDTO
{
    public HSGameState GameState { get; set; }
    public HSPlayerState PlayerState { get; set; }
    public HSOpponentState OpponentState { get; set; }
}

public class HSGameState
{
    public Guid MatchGUID { get; set; }
    public List<HSGameUserInfo> Players { get; set; }
    public Int64 CurrentTurnUid { get; set; }
    public bool IsGameOver { get; set; }
    public Int64? WinnerUid { get; set; }
    public DateTime LastActionTime { get; set; }
    public int TurnCount { get; set; }
}

// 게임 종료 요청을 위한 DTO
public class FinishGameRequest
{
    public Guid MatchGUID { get; set; }
    public Int64? WinnerUid{get;set;}
}

public class FinishGameResponse : ErrorCodeDTO
{
    public Int64? WinnerUid { get; set; }
}

// 카드 드로우 관련
public class DrawCardRequest
{
    public Guid MatchGUID { get; set; }
}

public class DrawCardResponse : ErrorCodeDTO
{
    public CardInfo? DrawnCard { get; set; }
}

// 카드 사용 관련
public class PlayCardRequest
{
    public Guid MatchGUID { get; set; }
    public int CardId { get; set; }
}

public class PlayCardResponse : ErrorCodeDTO
{
    public bool Success { get; set; }
}

// 공격 관련
public class AttackRequest
{
    public Guid MatchGUID { get; set; }
    public int AttackerCardId { get; set; }
    public int TargetCardId { get; set; }
}

public class AttackResponse : ErrorCodeDTO
{
    public bool Success { get; set; }
    public int DamageDealt { get; set; }
}

// 턴 종료 관련
public class EndTurnRequest
{
    public Guid MatchGUID { get; set; }
}

public class EndTurnResponse : ErrorCodeDTO
{
    public Int64 NextTurnUid { get; set; }
}

public class HSPlayerState
{
    public Int64 AccountUid { get; set; }
    public Dictionary<int, CardInfo> HandCardList { get; set; } = new Dictionary<int, CardInfo>();
    public Dictionary<int, CardInfo> FieldCardList { get; set; } = new Dictionary<int, CardInfo>();
    public int DeckCount { get; set; }
    public bool HasDrawnCardThisTurn { get; set; } = false;
    public List<int> AttackCardList { get; set; } = new List<int>(); // 이번턴에서 공격했었던 카드
    public List<int> UseCardList { get; set; } = new List<int>(); // 현재까지 덱에서 꺼내온 카드

    public HSOpponentState ToHSOppentState()
    {
        return new HSOpponentState
        {
            AccountUid = this.AccountUid,
            FieldCardList = this.FieldCardList,
            DeckCount = this.DeckCount
        };
    }
}

public class HSOpponentState
{
    public Int64 AccountUid { get; set; }
    public Dictionary<int, CardInfo> FieldCardList { get; set; } = new Dictionary<int, CardInfo>();
    public int DeckCount { get; set; }
}
public class HSGameResult
{
    public Guid MatchGUID { get; set; }
    public int MatchType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<Int64> PlayerUIDs { get; set; }
    public Int64? WinnerUid { get; set; }
    public int TurnCount { get; set; }
    public bool IsGameOver { get; set; }
}
