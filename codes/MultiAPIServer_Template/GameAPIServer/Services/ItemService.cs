using MatchAPIServer.DTO.DataLoad;
using MatchAPIServer.Models;
using MatchAPIServer.Models.GameDB;
using MatchAPIServer.Repository.Interfaces;
using MatchAPIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ZLogger;

namespace MatchAPIServer.Servicies
{
    public class ItemService : IItemService
    {
        readonly ILogger<ItemService> _logger;
        readonly IGameDb _gameDb;
        readonly IMasterDb _masterDb;

        public ItemService(ILogger<ItemService> logger, IGameDb gameDb, IMasterDb masterDb)
        {
            _logger = logger;
            _gameDb = gameDb;
            _masterDb = masterDb;
        }

        #region Character

        public async Task<(ErrorCode,List<UserCharInfo>)> GetCharList(int uid)
        {
            try
            {
                List<UserCharInfo> userCharInfoList = new();
                
                var charList =  await _gameDb.GetCharList(uid);
                foreach (var character in charList)
                {
                    UserCharInfo userCharInfo = new();
                    userCharInfo.CharInfo = character;
                    userCharInfo.RandomSkills = await _gameDb.GetCharRandomSkillInfo(uid, character.char_key);
                    userCharInfoList.Add(userCharInfo);
                }

                return (ErrorCode.None, userCharInfoList);
            }
            catch (System.Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.GetCharList] ErrorCode: {ErrorCode.CharListFailException}, Uid: {uid}");
                return (ErrorCode.CharListFailException, null);
            }
        }

        public async Task<ErrorCode> ReceiveChar(int uid, int charKey, int qty)
        {
            try
            {
                var charInfo = await _gameDb.GetCharInfo(uid, charKey);

                // 캐릭터가 없다면 추가
                if (charInfo == null)
                {
                    var rowCount = await _gameDb.InsertUserChar(uid, charKey, qty);
                    if (rowCount != 1)
                    {
                        return ErrorCode.CharReceiveFailInsert;
                    }
                }
                // 있다면 수량증가 or 레벨 업
                else
                {
                    // 레벨업 가능 레벨
                    var level = _masterDb._itemLevelList.FindLast(data => data.item_cnt <= charInfo.char_cnt + qty).level;
                    // 레벨업
                    if (level > charInfo.char_level)
                    {
                        var rowCount = await _gameDb.LevelUpChar(uid, charKey, level, charInfo.char_cnt+qty);
                        if (rowCount != 1)
                        {
                            return ErrorCode.CharReceiveFailLevelUP;
                        }
                    }
                    // 수량증가
                    else
                    {
                        var rowCount = await _gameDb.IncrementCharCnt(uid, charKey, qty);
                        if (rowCount != 1)
                        {
                            return ErrorCode.CharReceiveFailIncrementCharCnt;
                        }
                    }
                }

                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.ReceiveChar] ErrorCode: {ErrorCode.CharReceiveFailException}, Uid: {uid}, CharKey: {charKey}");
                return ErrorCode.CharReceiveFailException;
            }
        }

        public async Task<ErrorCode> SetCharCostume(int uid, int charKey, CharCostumeInfo costumeInfo)
        {
            try
            {
                if (costumeInfo.Head != 0)
                {
                    if (await _gameDb.GetCostumeInfo(uid, costumeInfo.Head) == null)
                    {
                        return ErrorCode.CharSetCostumeFailHeadNotExist;
                    }
                }

                if (costumeInfo.Face != 0)
                {
                    if (await _gameDb.GetCostumeInfo(uid, costumeInfo.Face) == null)
                    {
                        return ErrorCode.CharSetCostumeFailFaceNotExist;
                    }
                }

                if (costumeInfo.Hand != 0)
                {
                    if (await _gameDb.GetCostumeInfo(uid, costumeInfo.Hand) == null)
                    {
                        return ErrorCode.CharSetCostumeFailHandNotExist;
                    }
                }

                JsonObject json = new()
                {
                    { "head", costumeInfo.Head },
                    { "face", costumeInfo.Face },
                    { "hand", costumeInfo.Hand }
                };

                var rowCount = await _gameDb.SetCharCostume(uid, charKey, json.ToJsonString());
                if (rowCount != 1)
                {
                    return ErrorCode.CharSetCostumeFailUpdate;
                }

                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.SetCharCostume] ErrorCode: {ErrorCode.CharSetCostumeFailException}, Uid: {uid}, CharKey: {charKey}");
                return ErrorCode.CharSetCostumeFailException;
            }
        }

        #endregion

        #region Skin

        public async Task<(ErrorCode, IEnumerable<GdbUserSkinInfo>)> GetSkinList(int uid)
        {
            try
            {
                return (ErrorCode.None, await _gameDb.GetSkinList(uid));
            }
            catch(System.Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.GetSkinList] ErrorCode: {ErrorCode.SkinListFailException}, Uid: {uid}");
                return (ErrorCode.SkinListFailException, null);
            }
        }

        public async Task<ErrorCode> ReceiveSkin(int uid, int skinKey)
        {
            try
            {
                var skinInfo = await _gameDb.GetSkinInfo(uid, skinKey);

                // 스킨이 있다면 에러
                if (skinInfo != null)
                {
                    return ErrorCode.SkinReceiveFailAlreadyOwn;
                }
                // 스킨 추가
                var rowCount = await _gameDb.InsertUserSkin(uid, skinKey);
                if (rowCount != 1)
                {
                    return ErrorCode.SkinReceiveFailInsert;
                }

                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.ReceiveSkin] ErrorCode: {ErrorCode.SkinReceiveFailException}, Uid: {uid}, SkinKey: {skinKey}");
                return ErrorCode.SkinReceiveFailException;
            }
        }

        #endregion

        #region Costume

        public async Task<(ErrorCode,IEnumerable<GdbUserCostumeInfo>)> GetCostumeList(int uid)
        {
            try
            {
                return (ErrorCode.None, await _gameDb.GetCostumeList(uid));
            }
            catch (System.Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.GetCostumeList] ErrorCode: {ErrorCode.CostumeListFailException}, Uid: {uid}");
                return (ErrorCode.CostumeListFailException, null);
            }
        }

        public async Task<ErrorCode> ReceiveCostume(int uid, int costumeKey, int qty)
        {
            try
            {
                var costumeInfo = await _gameDb.GetCostumeInfo(uid, costumeKey);

                // 코스튬이 없다면 추가
                if (costumeInfo == null)
                {
                    var rowCount = await _gameDb.InsertUserCostume(uid, costumeKey, qty);
                    if (rowCount != 1)
                    {
                        return ErrorCode.CostumeReceiveFailInsert;
                    }
                }
                // 있다면 수량증가 or 레벨 업
                else
                {
                    // 레벨업 가능한 레벨
                    var level = _masterDb._itemLevelList.FindLast(data => data.item_cnt <= costumeInfo.costume_cnt + qty).level;
                    // 레벨업
                    if (level > costumeInfo.costume_level)
                    {
                        var rowCount = await _gameDb.LevelUpCostume(uid, costumeKey, level, costumeInfo.costume_cnt + qty);
                        if (rowCount != 1)
                        {
                            return ErrorCode.CostumeReceiveFailLevelUP;
                        }
                    }
                    // 수량증가
                    else
                    {
                        var rowCount = await _gameDb.IncrementCostumeCnt(uid, costumeKey, qty);
                        if (rowCount != 1)
                        {
                            return ErrorCode.CostumeReceiveFailIncrementCharCnt;
                        }
                    }
                }

                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.ReceiveCostume] ErrorCode: {ErrorCode.CostumeReceiveFailException}, Uid: {uid}, CostumeKey: {costumeKey}");
                return ErrorCode.CostumeReceiveFailException;
            }
        }

        #endregion

        #region Food

        public async Task<(ErrorCode,IEnumerable<GdbUserFoodInfo>)> GetFoodList(int uid)
        {
            try
            {
                return (ErrorCode.None, await _gameDb.GetFoodList(uid));
            }
            catch (System.Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.GetFoodList] ErrorCode: {ErrorCode.FoodListFailException}, Uid: {uid}");
                return (ErrorCode.FoodListFailException, null);
            }
        }

        public async Task<ErrorCode> ReceiveFood(int uid, int foodKey, int qty)
        {
            try
            {
                var foodInfo = await _gameDb.GetFoodInfo(uid, foodKey);

                // 음식이 없다면 추가
                if (foodInfo == null)
                {
                    var rowCount = await _gameDb.InsertUserFood(uid, foodKey, qty);
                    if (rowCount != 1)
                    {
                        return ErrorCode.FoodReceiveFailInsert;
                    }
                }
                // 있다면 수량증가
                else
                {
                    var rowCount = await _gameDb.IncrementFoodQty(uid, foodKey, qty);
                    if (rowCount != 1)
                    {
                        return ErrorCode.FoodReceiveFailIncrementFoodQty;
                    }
                }

                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.ReceiveFood] ErrorCode: {ErrorCode.FoodReceiveFailException}, Uid: {uid}, FoodKey: {foodKey}");
                return ErrorCode.FoodReceiveFailException;
            }
        }

        public async Task<ErrorCode> ReceiveFoodGear(int uid, int foodGearKey, int gearQty)
        {
            try
            {
                var foodKey = foodGearKey - 100;
                var foodInfo = await _gameDb.GetFoodInfo(uid, foodKey);

                // 음식이 없다면 추가
                if (foodInfo == null)
                {
                    var rowCount = await _gameDb.InsertUserFood(uid, foodKey, 0, gearQty);
                    if (rowCount != 1)
                    {
                        return ErrorCode.FoodGearReceiveFailInsert;
                    }
                }
                // 있다면 수량증가
                else
                {
                    var rowCount = await _gameDb.IncrementFoodGearQty(uid, foodKey, gearQty);
                    if (rowCount != 1)
                    {
                        return ErrorCode.FoodGearReceiveFailIncrementFoodGear;
                    }
                }

                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.ReceiveFoodGear] ErrorCode: {ErrorCode.FoodGearReceiveFailException}, Uid: {uid}, FoodGearKey: {foodGearKey}");
                return ErrorCode.FoodGearReceiveFailException;
            }
        }

        #endregion

        public async Task<(ErrorCode,List<RewardData>)> ReceiveOneGacha(int uid, int gachaKey)
        {
            try
            {
                List<RewardData> rewardDatas = [];

                // 가챠 정보 가져오기
                var gacha = _masterDb._gachaRewardList.Find(item => item.gachaRewardInfo.gacha_reward_key == gachaKey);
                var gachaInfo = gacha.gachaRewardInfo;

                // 가챠 확률을 위한 총합
                var totalPercent = gachaInfo.char_prob_percent
                                 + gachaInfo.skin_prob_percent 
                                 + gachaInfo.costume_prob_percent 
                                 + gachaInfo.food_prob_percent 
                                 + gachaInfo.food_gear_prob_percent;

                // 가챠 확률
                int[] probs = { gachaInfo.char_prob_percent,
                                gachaInfo.skin_prob_percent,
                                gachaInfo.costume_prob_percent,
                                gachaInfo.food_prob_percent,
                                gachaInfo.food_gear_prob_percent };

                // 가챠 타입
                string[] types = { "char", "skin", "costume", "food", "food_gear" };

                // 가챠의 뽑기횟수만큼 반복
                for (int i = 0; i < gachaInfo.gacha_count; i++)
                {
                    //숫자를 뽑고 확률 배열에 따라 타입 고르기
                    var randomPoint = new Random().Next(1, totalPercent + 1);
                    for (int j = 0; j < probs.Length; j++)
                    {
                        if (randomPoint <= probs[j])
                        {
                            // 정해진 타입의 보상들 중 다시 랜덤 뽑기
                            var rewards = gacha.gachaRewardList.FindAll(item => item.reward_type == types[j]);
                            var randomIndex = new Random().Next(0, rewards.Count);
                            var reward = rewards[randomIndex];
                            // 보상 지급
                            var errorCode = await ReceiveReward(uid, reward);
                            if (errorCode != ErrorCode.None)
                            {
                                return (errorCode, null);
                            }
                            rewardDatas.Add(reward);
                            break;
                        }
                        randomPoint -= probs[j];
                    }
                }
                
                return (ErrorCode.None, rewardDatas);
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.ReceiveOneGacha] ErrorCode: {ErrorCode.GachaReceiveFailException}, Uid: {uid}, GachaKey: {gachaKey}");
                return (ErrorCode.GachaReceiveFailException, null);
            }
        }

        public async Task<ErrorCode> ReceiveReward(int uid, RewardData reward)
        {
            int rowCount;
            var errorCode = ErrorCode.None;
            try
            {
                switch (reward.reward_type)
                {
                    case "money": //보석
                        rowCount = await _gameDb.UpdateUserjewelry(uid, reward.reward_qty);
                        if (rowCount != 1)
                        {
                            return ErrorCode.UserUpdateJewelryFailIncremnet;
                        }
                        break;
                    case "char": //캐릭터
                        errorCode = await ReceiveChar(uid, reward.reward_key, reward.reward_qty);
                        break;
                    case "skin": //스킨
                        errorCode = await ReceiveSkin(uid, reward.reward_key);
                        break;
                    case "costume": //코스튬
                        errorCode = await ReceiveCostume(uid, reward.reward_key, reward.reward_qty);
                        break;
                    case "food": //푸드
                        errorCode = await ReceiveFood(uid, reward.reward_key, reward.reward_qty);
                        break;
                    case "food_gear": // 푸드기어
                        errorCode = await ReceiveFoodGear(uid, reward.reward_key, reward.reward_qty);
                        break;
                }
                if(errorCode != ErrorCode.None)
                {
                    return errorCode;
                }

                return errorCode;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[GetReward] ErrorCode: {ErrorCode.GetRewardFailException}, Uid: {uid}");
                return ErrorCode.GetRewardFailException;
            }
        }
    }
}
