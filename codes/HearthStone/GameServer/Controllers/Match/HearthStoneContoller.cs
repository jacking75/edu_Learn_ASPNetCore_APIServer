using Microsoft.AspNetCore.Mvc;
using GameServer.Services.Interface;
using GameServer.Models.DTO;

namespace GameServer.Controllers;

[ApiController]
[Route("hearthstone")]
public class HearthStoneController : ControllerBase
{
    private readonly IHearthStoneService _hearthStoneService;

    public HearthStoneController(IHearthStoneService hearthStoneService)
    {
        _hearthStoneService = hearthStoneService;
    }

    [HttpPost("initgame")]
    public async Task<InitGameResponse> InitGame([FromHeader] HeaderDTO header, [FromBody] InitGameRequest request)
    {
        var response = new InitGameResponse();
        (response.Result, response.InitialCardList) = await _hearthStoneService.InitGame(header.AccountUid, request.MatchGUID);
        return response;
    }

    //[HttpPost("replacecards")]
    //public async Task<ReplaceCardsResponse> ReplaceInitialCards([FromHeader] HeaderDTO header, [FromBody] ReplaceCardsRequest request)
    //{
    //    var response = new ReplaceCardsResponse();
    //    (response.Result, response.InitialCardList) = await _hearthStoneService.ReplaceInitialCards(header.AccountUid, request.MatchGUID, request.ReplaceCardIndexList);
    //    return response;
    //}

    [HttpPost("state")]
    public async Task<GameStateResponse> State([FromHeader] HeaderDTO header, [FromBody]GameStateRequest request)
    {
        var response = new GameStateResponse();
        (response.Result, response.GameState, response.PlayerState, response.OpponentState) = await _hearthStoneService.GetGameState(header.AccountUid, request.MatchGuid);
        return response;
    }

    [HttpPost("finishgame")]
    public async Task<FinishGameResponse> FinishGame([FromHeader] HeaderDTO header, [FromBody] FinishGameRequest request)
    {
        var response = new FinishGameResponse();
        response.Result = await _hearthStoneService.FinishGame(header.AccountUid, request.MatchGUID, request.WinnerUid);
        return response;
    }

    [HttpPost("drawcard")]
    public async Task<DrawCardResponse> DrawCard([FromHeader] HeaderDTO header, [FromBody] DrawCardRequest request)
    {
        var response = new DrawCardResponse();
        (response.Result, response.DrawnCard) = await _hearthStoneService.DrawCard(header.AccountUid, request.MatchGUID);
        return response;
    }

    [HttpPost("playcard")]
    public async Task<PlayCardResponse> PlayCard([FromHeader] HeaderDTO header, [FromBody] PlayCardRequest request)
    {
        var response = new PlayCardResponse();
        (response.Result, response.Success) = await _hearthStoneService.PlayCard(header.AccountUid, request.MatchGUID, request.CardId);
        return response;
    }

    [HttpPost("attack")]
    public async Task<AttackResponse> Attack([FromHeader] HeaderDTO header, [FromBody] AttackRequest request)
    {
        var response = new AttackResponse();
        (response.Result, response.Success, response.DamageDealt) = await _hearthStoneService.Attack(
            header.AccountUid, request.MatchGUID, request.AttackerCardId, request.TargetCardId);
        return response;
    }

    [HttpPost("endturn")]
    public async Task<EndTurnResponse> EndTurn([FromHeader] HeaderDTO header, [FromBody] EndTurnRequest request)
    {
        var response = new EndTurnResponse();
        (response.Result, response.NextTurnUid) = await _hearthStoneService.EndTurn(header.AccountUid, request.MatchGUID);
        return response;
    }
}
