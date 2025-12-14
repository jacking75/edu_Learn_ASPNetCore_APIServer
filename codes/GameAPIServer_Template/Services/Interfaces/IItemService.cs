using GameAPIServer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces;

public interface IItemService
{
    public Task<(ErrorCode, List<RewardData>)> ReceiveOneGacha(Int64 uid, int gachaKey);

    public Task<ErrorCode> ReceiveReward(Int64 uid, RewardData reward);
  

}
