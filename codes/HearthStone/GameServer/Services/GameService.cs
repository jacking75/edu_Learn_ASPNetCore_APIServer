using Dapper;
using GameServer.Models;
using GameServer.Repository.Interface;
using GameServer.Services.Interface;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Transactions;
using ZLogger;
using GameServer.Repository;
using System.Reflection.Metadata.Ecma335;
using GameServer.Models.DTO;

namespace GameServer.Services;

public class GameService : IGameService
{
    readonly ILogger<GameService> _logger;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;
    int initCharKey;

    public GameService(ILogger<GameService> logger, IGameDb gameDb, IMasterDb masterDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
        initCharKey = 0; // 초기화할 캐릭터 키를 설정합니다.
    }
    async Task<int> CreateUserAsync(Int64 accountUid, IDbTransaction transaction) 
    {
        try
        {
            return await _gameDb.InsertUser(accountUid, transaction);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailException}, accountUid: {accountUid}");
            return 0;
        }
    }
    async Task<ErrorCode> CreateAttendance(Int64 accountUid, IDbTransaction transaction)
    {
        try
        {
            // 무료 출석 정보만 필터링된 리스트 가져오기
            var freeAttendanceList = _masterDb._initAttendanceInfoList;
            if (freeAttendanceList == null || freeAttendanceList.Count == 0)
            {
                return ErrorCode.None;
            }

            // 여러 출석 정보를 한 번에 삽입
            int insertedCount = await _gameDb.InsertAttendanceInfoList(accountUid, freeAttendanceList, transaction);

            if (insertedCount <= 0)
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailNoNickname}, uid : {accountUid}");

                return ErrorCode.CreateUserFailException;
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[CreateAttendance] ErrorCode: {ErrorCode.AttendanceInfoFailException}, Uid: {accountUid}");
            return ErrorCode.CreateUserFailException;
        }
    }

    async Task<ErrorCode> CreateMoney(Int64 accountUid, IDbTransaction transaction)
    {
        try
        {
             var result = _masterDb._initAssetInfoList;
            if (result == null || result.Count == 0)
            {
                return ErrorCode.None;
            }

            foreach (var info in result)
            { 
               if(await _gameDb.AddAssetInfo(accountUid, info.asset_name, info.asset_amount, transaction) < 1)
                {
                    _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailNoNickname}, uid : {accountUid}");

                    return ErrorCode.CreateUserFailException;
                }
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailNoNickname}, uid : {accountUid}");

            return ErrorCode.CreateUserFailException;
        }
    }

    async Task<ErrorCode> CreateItem(Int64 accountUid, IDbTransaction transaction)
    {
        try
        {
            // 무료 출석 정보만 필터링된 리스트 가져오기
            var itemList = _masterDb._initItemInfoList;
            if (itemList == null || itemList.Count == 0)
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailNoNickname}, uid : {accountUid}");

                return ErrorCode.CreateUserFailException;
            }

            foreach (var info in itemList)
            {
                if(await _gameDb.AddItemInfo(accountUid, info.item_id, info.item_cnt, transaction) < 1)
                {
                    _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailNoNickname}, uid : {accountUid}");

                    return ErrorCode.CreateUserFailException;
                }
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailNoNickname}, uid : {accountUid}");
            return ErrorCode.CreateUserFailException;
        }
    }

    async Task<ErrorCode> CreateMail(Int64 accountUid, IDbTransaction transaction) 
    {
        try 
        {
            var emailList = _masterDb._initMailInfoList;
            if(emailList == null || emailList.Count == 0)
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailNoNickname}, uid : {accountUid}");

                return ErrorCode.None;
            }

            foreach (var mailInfo in emailList)
            {
                if (await _gameDb.AddMailInfo(accountUid, mailInfo, transaction) < 1)
                {
                    _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailNoNickname}, uid : {accountUid}");

                    return ErrorCode.CreateUserFailException;
                }
            }

            return ErrorCode.None;

        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailException}, accountUid: {accountUid}");
            return ErrorCode.CreateUserFailException;
        }
    }

    public async Task<ErrorCode> InitNewUserGameData(Int64 accountUid)
    {
        var transaction = _gameDb.GetDbConnection().BeginTransaction();
        try
        {
            await CreateUserAsync(accountUid, transaction);

            var errorCode = await CreateAttendance(accountUid, transaction);
            if (errorCode != ErrorCode.None)
            {
                transaction.Rollback();
                return errorCode;
            }

            errorCode = await CreateMoney(accountUid, transaction);
            if (errorCode != ErrorCode.None)
            {
                transaction.Rollback();
                return errorCode;
            }

            errorCode = await CreateItem(accountUid, transaction);
            if (errorCode != ErrorCode.None)
            {
                transaction.Rollback();
                return errorCode;
            }

            errorCode = await CreateMail(accountUid, transaction);
            if (errorCode != ErrorCode.None)
            {
                transaction.Rollback();
                return errorCode;
            }

            transaction.Commit();
            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return ErrorCode.DatabaseError;
        }
    }

    public async Task<(ErrorCode, List<AssetInfo>)> GetAssetInfoList(Int64 accountUid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetAssetInfoList(accountUid));

        }
        catch (Exception ex)
        {
            // Log the exception
            return (ErrorCode.CurrencyLoadFail, null);
        }
    }
}
