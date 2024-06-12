using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FieldMonsterController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly IDataStorage _dataStorage;
        private readonly ILogger<FieldMonsterController> _logger;
        
        public FieldMonsterController(ILogger<FieldMonsterController> logger, IGameDb gameDb, IDataStorage dataStorage)
        {
            _logger = logger;
            _gameDb = gameDb;
            _dataStorage = dataStorage;
        }
        
        // 수습 기간 프로젝트임으로기능이 간단하게 구현되었습니다.
        [HttpPost]
        public async Task<FieldMonsterResponse> FieldMonsterPost(FieldMonsterRequest request)
        {
            var response = new FieldMonsterResponse();
            
            var rand = new Random();
            var randValue = rand.Next(1, 7); // 기획 데이터 UID 1~6까지 존재함.
            var monster = _dataStorage.GetMonsterInfo(randValue);
            if (monster == null)
            {
                response.Result = ErrorCode.DataStorageReadMonsterFail;
                _logger.ZLogError($"{nameof(FieldMonsterPost)} ErrorCode : {response.Result}");
                return response;
            }
            
            response.MonsterID = randValue;
            response.Att = monster.Att;
            response.Def = monster.Def;
            response.Level = monster.Level;
            response.Name = monster.MonsterName;
            response.Type = monster.Type;
            response.HP = monster.HP;
            response.StarCount = monster.StarCount;
            
            return response;
        }
    }
}