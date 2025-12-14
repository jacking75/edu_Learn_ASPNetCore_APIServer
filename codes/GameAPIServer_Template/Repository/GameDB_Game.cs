using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using GameAPIServer.Repository.Interfaces;
using GameAPIServer.Models;
using SqlKata.Execution;
using ZLogger;

namespace GameAPIServer.Repository;

public partial class GameDb : IGameDb
{
    public async Task<(ErrorCode, Int64)> CreateAccount(string userID, string pw)
    {
        try
        {
            var saltValue = Security.SaltString();
            var hashingPassword = Security.MakeHashingPassWord(saltValue, pw);

            var uid = await _queryFactory.Query("account")
                                .InsertGetIdAsync<Int64>(new
                                {
                                    user_id = userID,
                                    salt_value = saltValue,
                                    pw = hashingPassword,
                                    create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                });

            _logger.ZLogDebug(
            $"[CreateAccount] email: {userID}, salt_value : {saltValue}, hashed_pw:{hashingPassword}");

            var errorCode =ErrorCode.None;
            if(uid <= 0)
            {
                errorCode = ErrorCode.CreateAccountFailInsert;
            }

            return (errorCode, uid);
        }
        catch (Exception ex)
        {
            _logger.ZLogError(
            $"[HiveDb.CreateAccount] ErrorCode: {ErrorCode.CreateAccountFailException}, {ex}");
            return (ErrorCode.CreateAccountFailException, 0);
        }
    }

    public async Task<(ErrorCode, Int64)> VerifyUser(string userID, string pw)
    {
        try
        {
            var userInfo = await _queryFactory.Query("account")
                                    .Where("user_id", userID)
                                    .FirstOrDefaultAsync<Account>();

            if (userInfo is null)
            {
                return (ErrorCode.LoginFailUserNotExist, 0);
            }

            var hashingPassword = Security.MakeHashingPassWord(userInfo.salt_value, pw);
            if (userInfo.pw != hashingPassword)
            {
                return (ErrorCode.LoginFailPwNotMatch, 0);
            }

            return (ErrorCode.None, userInfo.player_id);
        }
        catch (Exception ex)
        {
            _logger.ZLogError(
            $"[HiveDb.VerifyUser] ErrorCode: {ErrorCode.LoginFailException}, {ex}");
            return (ErrorCode.LoginFailException, 0);
        }
    }
        

    public async Task<int> InsertInitMoneyInfo(Int64 uid, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user_money").InsertAsync(
             new
             {
                 uid = uid
             }, transaction);
    }

    public async Task<int> InsertInitAttendance(Int64 uid, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user_attendance").InsertAsync(
             new
             {
                 uid = uid,
                 recent_attendance_dt = DateTime.Now.AddDays(-1)
             }, transaction);
    }

    
    
}