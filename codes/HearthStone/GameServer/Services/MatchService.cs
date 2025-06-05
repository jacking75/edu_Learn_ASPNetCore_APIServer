using GameServer.Repository;
using GameServer.Repository.Interface;
using GameServer.Services.Interface;
using System.Text.Json;
using System.Threading.Tasks;
using ZLogger;
using GameServer.Models.DTO;
using System.Net.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace GameServer.Services;

public class MatchService : IMatchService
{
    private readonly ILogger<MatchService> _logger;
    private readonly IMemoryDb _memoryDb;
    private readonly IGameDb _gameDb;
    private IHttpClientFactory _httpClientFactory;

    public MatchService(
         ILogger<MatchService> logger,
         IMemoryDb memoryDb,
         IGameDb gameDb,
         IHttpClientFactory httpClientFactory)

    {
        _logger = logger;
        _memoryDb = memoryDb;
        _gameDb = gameDb;
        _httpClientFactory = httpClientFactory;
    }

    async Task<bool> CheckMainDeck(Int64 accountUid)
    {
        try
        {
            var deckInfo = await _gameDb.GetMainDeckInfo(accountUid);
            if (deckInfo == null)
            {
                _logger.ZLogError($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.DeckLoadFail}");
                return false;
            }

            return true;
        }
        catch
        {
            _logger.ZLogError($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.MatchStatusCheckFailException}");

            return false;
        }
    }

    public async Task<ErrorCode> AddUser(Int64 accountUid)
    {
        try
        {
            if(await CheckMainDeck(accountUid) == false)
            {
                return ErrorCode.MatchAddUserFailAboutDeck;
            }

            var client = _httpClientFactory.CreateClient("MatchServer");
            var endpoint = "match/add"; 

            var result = await client.PostAsJsonAsync(endpoint, new MatchAddReqeust {AccountUid = accountUid });
            if (result == null)
                return ErrorCode.MatchCancelFailException;
            
            var readjson = await result.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(readjson))
            {
                _logger.ZLogError($"Server Error: {readjson}");
                return ErrorCode.MatchCancelFailException;
            }
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var response = JsonSerializer.Deserialize<MatchAddResponse>(readjson, options);
            return response.Result;
        }
        catch(Exception ex)
        {
            _logger.ZLogError($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.MatchStatusCheckFailException}");

            return ErrorCode.MatchRequestFailException;
        }
    }
    public async Task<(ErrorCode, Guid)> GetMatchGUID(Int64 accountUid) 
    {
        try
        {
            (var result, var guid) = await _memoryDb.GetMatchGUID(accountUid);

            return (result) ? (ErrorCode.None, guid) : (ErrorCode.MatchingWaiting, Guid.Empty);
        }
        catch
        {
            _logger.ZLogError($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.MatchStatusCheckFailException}");

            return (ErrorCode.MatchingWaiting, Guid.Empty);
        }
    }

    public async Task<(ErrorCode, HSGameInfo?)> GetMatch(Guid matchGuid, Int64 accountUid) 
    {
        try
        {
            var gameInfo = await _memoryDb.GetMatchInfo(matchGuid);
           if (gameInfo == null)
            {
                gameInfo = await _memoryDb.UpdateMatchInfo(matchGuid, accountUid);
            }

            return (gameInfo != null) ? (ErrorCode.None, gameInfo) : (ErrorCode.MatchStatusCheckFailException, null);
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.MatchStatusCheckFailException}");

            return (ErrorCode.MatchStatusCheckFailException, null);
        }
    }
}