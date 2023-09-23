//TODO 샘플용 코드. 사용하지 않으면 삭제해도 됨.
using APIServer.Model.DAO;
using APIServer.Model.DTO;
using APIServer.Repository;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CharacterList : Controller
{
    private readonly IGameDb _gameDb;
    private readonly IMemoryDb _memoryDb;
    private readonly ILogger<CharacterList> _logger;

    public CharacterList(ILogger<CharacterList> logger, IGameDb gameDb, IMemoryDb memoryDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _memoryDb = memoryDb;
    }

    [HttpPost]
    public async Task<PkCharacterListRes> Post(PkCharacterListReq request)
    {
        RdbAuthUserData userInfo = (RdbAuthUserData)HttpContext.Items[nameof(RdbAuthUserData)]!;


        PkCharacterListRes response = new();

        /*(ErrorCode errorCode, System.Collections.Generic.IEnumerable<CharacterInfo> characters) = await _gameDb.GetCharacterList(userInfo.AccountId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogError($"[CharacterList] Error : {errorCode}");

            response.Result = errorCode;
            return response;
        }


        foreach (CharacterInfo character in characters)
        {
            CharacterInfo characterInfo = new()
            {
                Level = character.Level,
                NickName = character.NickName
            };

            response.CharacterInfoList.Add(characterInfo);
        }*/

        return response;
    }



    public class PkCharacterListReq
    {
        public string Email { get; set; }
        public string AuthToken { get; set; }
    }

    
    public class PkCharacterListRes
    {
        public List<CharacterInfo> CharacterInfoList { get; set; } = new();
        public ErrorCode Result { get; set; } = ErrorCode.None;
    }

    public class CharacterInfo
    {
        public int Level { get; set; }
        public string NickName { get; set; }
    }

    public class CharacterLookInfo
    {
        public long CharacterId { get; set; }

        public int Eye { get; set; }
        public int HairStyle { get; set; }
        public int Mustache { get; set; }
        public int Cloak { get; set; }
        public int Pants { get; set; }
        public int Dress { get; set; }
        public int Armor { get; set; }
        public int Helmet { get; set; }
    }

}
