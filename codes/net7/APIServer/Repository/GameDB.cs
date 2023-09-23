//TODO 사용하지 않는 코드 정리하기
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using APIServer.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;
using static APIServer.Controllers.CharacterList;

namespace APIServer.Services;

public class GameDb : IGameDb
{
    readonly ILogger<GameDb> _logger;
    readonly IOptions<DbConfig> _dbConfig;
        
    IDbConnection _dbConn;
    SqlKata.Compilers.MySqlCompiler _compiler;
    QueryFactory _queryFactory;
    
    public GameDb(ILogger<GameDb> logger, IOptions<DbConfig> dbConfig)
    {
        _dbConfig = dbConfig;
        _logger = logger;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, _compiler);
    }

    public void Dispose()
    {
        Close();
    }


    public async Task<Tuple<ErrorCode, Int64>> InsertCharacter(Int64 accountId, string nickName)
    {
        try
        {
            var characterId = await _queryFactory.Query("character").InsertGetIdAsync<int>(new
            {
                AccountId = accountId,
                Nickname = nickName,
                Exp = 100,
                Level = 1,
                Hp = 50,
                Mp = 60,
                LocationX = 100,
                LocationY = 200,
                Gold = 10000
            });
                       
            return new Tuple<ErrorCode, Int64>(ErrorCode.None, characterId);
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex,
                $"[GameDb.InsertCharacter] ErrorCode : {ErrorCode.CreateCharacterFailException}");
            return new Tuple<ErrorCode, Int64>(ErrorCode.CreateCharacterFailException, -1);
        }
    }

    public async Task<ErrorCode> InsertCharacterItem(Int64 characterId)
    {
        try
        {
            var count = await _queryFactory.Query("character_item").InsertAsync(new
            {
                CharacterId = characterId,
                Dress = 1,
                Pants = 1,
                HairStyle = 1,
                Mustache = 1,
                Cloak = 1,
                Helmet = 1,
                Armor = 1
            });

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[GameDb.InsertCharacterDress ] ErrorCode : {ErrorCode.CreateCharacterRollbackFail}");
            return ErrorCode.CreateCharacterRollbackFail;
        }       
    }

    public async Task<ErrorCode> DeleteCharacter(string nickName)
    {        
        try
        {
            await _queryFactory.Query("character").Where("NickName", nickName).DeleteAsync();
            
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[GameDb.DeleteCreateCharacter ] ErrorCode : {ErrorCode.CreateCharacterRollbackFail}");
            return ErrorCode.CreateCharacterRollbackFail;
        }
    }

    //캐릭터 아이템 삭제 
    public async Task<ErrorCode> DeleteCharacterItem(Int64 characterId)
    {
        try
        {
            await _queryFactory.Query("character_item").Where("CharacterId", characterId).DeleteAsync();
        
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[GameDb.DeleteCreateCharacter ] ErrorCode : {ErrorCode.CreateCharacterRollbackFail}");
            return ErrorCode.CreateCharacterRollbackFail;
        }
    }

    /*public async Task<Tuple<ErrorCode, List<CharacterInfo>>> GetCharacterList(Int64 accountId)
    {
        try
        {
            var characterList = await _queryFactory.Query("character")
                .Select("Level", "NickName")
                .Where("AccountId", accountId)
                .GetAsync<CharacterInfo>();

            return new Tuple<ErrorCode, IEnumerable<CharacterInfo>>(ErrorCode.None, characterList);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[GameDb.GetCharacterList] ErrorCode : {ErrorCode.GetCharacterListFail}");
            return new Tuple<ErrorCode, IEnumerable<CharacterInfo>>(ErrorCode.GetCharacterListFail, null);
        }
        return new Tuple<ErrorCode, List<CharacterInfo>>(ErrorCode.None, null);
    }   */
    
   

    private void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.GameDb);

        _dbConn.Open();
    }

    private void Close()
    {
        _dbConn.Close();
    }
}