using GameServer.Models.DTO;
using GameServer.Services.Interface;
using GameServer.Repository.Interface;
using ZLogger;
using GameServer.Models;

namespace GameServer.Services;

public class DataLoadService : IDataLoadService
{
    readonly ILogger<DataLoadService> _logger;
    readonly IMasterDb _masterDb;
    readonly IGameService _gameService;
    readonly IItemService _itemService;
    readonly IAttendanceService _attendanceService;
    readonly IMailService _mailService;

    public DataLoadService(ILogger<DataLoadService> logger, IMasterDb masterDb, IGameService gameService, IItemService itemService, IAttendanceService attendanceService, IMailService mailService)
    {
        _logger = logger;
        _masterDb = masterDb;
        _gameService = gameService;
        _itemService = itemService;
        _attendanceService = attendanceService;
        _mailService = mailService;
    }

    public async Task<DataLoadResponse> LoadUserData(Int64 accountUid) 
    {
        DataLoadResponse response = new();

        (response.Result, response.CurrencyList) = await _gameService.GetAssetInfoList(accountUid);
        if (response.Result != ErrorCode.None)
        {
            return response;
        }

        (response.Result, response.ItemInfoList) = await _itemService.GetItemInfoList(accountUid);
        if (response.Result != ErrorCode.None)
        {
            return response;
        }

        (response.Result, response.DeckList) = await _itemService.GetDeckInfoList(accountUid);
        if (response.Result != ErrorCode.None)
        {
            return response;
        }

        (response.Result, response.MailList) = await _mailService.GetMailInfoList(accountUid);
        if (response.Result != ErrorCode.None)
        {
            return response;
        }

        (response.Result, response.AttendanaceList) = await _attendanceService.GetAttendanceInfoList(accountUid);
        if (response.Result != ErrorCode.None)
        {
            return response;
        }
       
        return response;
    }

    public TableLoadReponse LoadTableData()
    {
        TableLoadReponse response = new TableLoadReponse
        {
            Result = ErrorCode.None,
            ItemAbilityInfoList = _masterDb._abilityInfoList?.Select(item => new MdbAbilityInfo
            {
                ability_key = item.ability_key,
                ability_type = item.ability_type,
                ability_value = item.ability_value,
            }).ToList() ?? new List<MdbAbilityInfo>()
        };

        return response;
    }
}
