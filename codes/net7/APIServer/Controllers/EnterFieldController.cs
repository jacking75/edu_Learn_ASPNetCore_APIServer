//TODO 샘플용 코드. 사용하지 않으면 삭제해도 됨.
using APIServer.Model.DAO;
using APIServer.Model.DTO;
using APIServer.Repository;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class EnterField : ControllerBase
{
    private readonly IGameDb _gameDb;
    private readonly IMemoryDb _memoryDb;
    private readonly ILogger<EnterField> _logger;

    public EnterField(ILogger<EnterField> logger, IGameDb gameDb, IMemoryDb memoryDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _memoryDb = memoryDb;
    }

    [HttpPost]
    public EnterFieldRes Post(EnterFieldReq request)
    {
        RdbAuthUserData userInfo = (RdbAuthUserData)HttpContext.Items[nameof(RdbAuthUserData)]!;

        EnterFieldRes response = new();

        return response;
    }
}
