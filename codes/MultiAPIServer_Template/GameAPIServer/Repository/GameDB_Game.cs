using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using HiveAPIServer.Models.GameDB;
using HiveAPIServer.Repository.Interfaces;
using SqlKata.Execution;

namespace HiveAPIServer.Services;

public partial class GameDb : IGameDb
{
    public async Task<IEnumerable<GdbMiniGameInfo>> GetMiniGameList(int uid)
    {
        return await _queryFactory.Query("user_minigame")
            .Where("uid", uid)
            .OrderBy("game_key")
            .GetAsync<GdbMiniGameInfo>();
    }

    public async Task<int> InsertInitGameList(int uid, int initCharKey, IDbTransaction transaction)
    {
        var now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        return await _queryFactory.Query("user_minigame").InsertAsync(new[] { "uid", "game_key", "create_dt", "play_char_key" }, new[]
        {
            new object[]{uid,1,now, initCharKey},
            new object[]{uid,2,now, initCharKey},
            new object[]{uid,3,now, initCharKey},
        }, transaction);
    }

    public async Task<int> InsertMiniGame(int uid, int initCharKey, int gameKey)
    {
        return await _queryFactory.Query("user_minigame").InsertAsync(
            new
            {
                uid = uid,
                play_char_key = initCharKey,
                game_key = gameKey,
                create_dt = DateTime.Now
            });
    }

    public async Task<GdbMiniGameInfo> GetMiniGameInfo(int uid, int gameKey)
    {
        return await _queryFactory.Query("user_minigame").Where("uid", uid)
                                                .Where("game_key", gameKey)
                                                .FirstOrDefaultAsync<GdbMiniGameInfo>();
    }

    public async Task<int> UpdateBestscore(int uid, int gameKey, int score)
    {
        return await _queryFactory.Query("user_minigame").Where("uid", uid)
                                                .Where("game_key", gameKey)
                                                .Where("bestscore", "<", score)
                                                .UpdateAsync(new
                                                {
                                                    bestscore = score,
                                                    bestscore_cur_season = score,
                                                    new_record_dt = DateTime.Now,
                                                    recent_play_dt = DateTime.Now
                                                });
    }

    public async Task<int> UpdateBestscoreCurSeason(int uid, int gameKey, int score)
    {
        return await _queryFactory.Query("user_minigame").Where("uid", uid)
                                                .Where("game_key", gameKey)
                                                .Where("bestscore_cur_season", "<", score)
                                                .UpdateAsync(new
                                                {
                                                    bestscore_cur_season = score,
                                                    recent_play_dt = DateTime.Now
                                                });
    }

    public async Task<int> UpdateUserTotalBestScore(int uid)
    {
        return await _queryFactory.Query("user").Where("uid",uid).UpdateAsync(new
        {
            total_bestscore = _queryFactory.Query("user_minigame").Where("uid", uid)
                                                             .Sum<int>("bestscore")
        });
    }

    public async Task<int> UpdateUserTotalBestScoreCurSeason(int uid)
    {
        return await _queryFactory.Query("user").Where("uid", uid).UpdateAsync(new
        {
            total_bestscore_cur_season = _queryFactory.Query("user_minigame").Where("uid", uid)
                                                             .SumAsync<int>("bestscore_cur_season")
        });
    }

    public async Task<int> GetTotalBestscore(int uid)
    {
        return await _queryFactory.Query("user").Where("uid", uid).Select("total_bestscore").FirstOrDefaultAsync<int>();
    }

    public async Task<int> UpdateRecentPlayDt(int uid, int gameKey)
    {
        return await _queryFactory.Query("user_minigame").Where("uid", uid)
                                                .Where("game_key", gameKey)
                                                .UpdateAsync(new
                                                {
                                                    recent_play_dt = DateTime.Now
                                                });
    }

    public async Task<int> InsertInitCharacter(int uid, int initCharKey, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user_char").InsertAsync(
            new
            {
                uid = uid,
                char_key = initCharKey,
                create_dt = DateTime.Now,
                costume_json = "{\"face\" : 0, \"hand\" : 0, \"head\" : 0}"
            }, transaction);
    }

    public async Task<int> InsertInitMoneyInfo(int uid, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user_money").InsertAsync(
             new
             {
                 uid = uid
             }, transaction);
    }

    public async Task<int> InsertInitAttendance(int uid, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user_attendance").InsertAsync(
             new
             {
                 uid = uid,
                 recent_attendance_dt = DateTime.Now.AddDays(-1)
             }, transaction);
    }

    public async Task<int> UpdateMiniGamePlayChar(int uid, int gameKey, int charKey)
    {
        return await _queryFactory.Query("user_minigame").Where("uid", uid)
                                                .Where("game_key", gameKey)
                                                .UpdateAsync(new
                                                {
                                                    play_char_key = charKey
                                                });
    }

    public async Task<int> FoodDecrement(int uid, int foodKey, int foodQty)
    {
        return await _queryFactory.Query("user_food").Where("uid", uid)
                                                .Where("food_key", foodKey)
                                                .Where("food_qty", ">=", foodQty)
                                                .DecrementAsync("food_qty", foodQty);
    }
}