using GameAPIServer.Servicies.Interfaces;
using GameAPIServer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ZLogger;
using GameAPIServer.Repository.Interfaces;

namespace GameAPIServer.Servicies;

public class GameService :IGameService
{
    readonly ILogger<GameService> _logger;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;
    readonly IMemoryDb _memoryDb;


    public GameService(ILogger<GameService> logger, IGameDb gameDb, IMasterDb masterDb, IMemoryDb memoryDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
        _memoryDb = memoryDb;
    }


    public async Task<ErrorCode> InitNewUserGameData(Int64 uid)
    {
        var transaction = _gameDb.GDbConnection().BeginTransaction();
        try
        {
            var rowCount = await _gameDb.InsertInitMoneyInfo(uid, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return ErrorCode.InitNewUserGameDataFailMoney;
            }

            rowCount = await _gameDb.InsertInitAttendance(uid, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return ErrorCode.InitNewUserGameDataFailAttendance;
            }

            transaction.Commit();
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            _logger.ZLogError(e,
                $"[Game.InitNewUserGameData] ErrorCode: {ErrorCode.InitNewUserGameDataFailException}, uid : {uid}");
            return ErrorCode.GameSetNewUserListFailException;
        }
        finally
        {
            transaction.Dispose();
        }
    }

    

}
