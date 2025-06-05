using Microsoft.AspNetCore.Mvc;
using GameServer.Models.DTO;
using GameServer.Services.Interface;

namespace GameServer.Controllers;

[ApiController]
[Route("contents")]
public class DataLoadController : ControllerBase
{
    readonly ILogger<DataLoadController> _logger;
    readonly IDataLoadService _dataLoadService;

    public DataLoadController(ILogger<DataLoadController> logger, IDataLoadService dataLoadService)
    {
        _logger = logger;
        _dataLoadService = dataLoadService;
    }

    [HttpPost("dataload")]
    public async Task<DataLoadResponse> GetUserDataLoad([FromHeader] HeaderDTO header)
    {
        DataLoadResponse response = await _dataLoadService.LoadUserData(header.AccountUid);
        return response;
    }

    [HttpPost("table")]
    public async Task<TableLoadReponse> GetTableLoad([FromHeader] HeaderDTO header)
    {
        TableLoadReponse response = _dataLoadService.LoadTableData();
        return response;
    }
}
