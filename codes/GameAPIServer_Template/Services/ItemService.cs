using APIServer.Models;
using APIServer.Repository.Interfaces;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies
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
