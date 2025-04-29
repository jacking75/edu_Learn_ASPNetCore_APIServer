using GameAPIServer.DTO.Ranking;
using GameAPIServer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameAPIServer.Repository.Interfaces;

public interface IMemoryDb
{
    public Task<ErrorCode> RegistUserAsync(string token, int uid);
    public Task<ErrorCode> CheckUserAuthAsync(string id, string authToken);
    public Task<(bool, RdbAuthUserData)> GetUserAsync(string id);
    public Task<bool> LockUserReqAsync(string key);
    public Task<bool> UnLockUserReqAsync(string key);
    public Task<ErrorCode> DelUserAuthAsync(int uid);
    public  Task<ErrorCode> SetUserScore(int uid, int score);
    public  Task<(ErrorCode, List<RankData>)> GetTopRanking();
    public Task<(ErrorCode, Int64)> GetUserRankAsync(int uid);
}
