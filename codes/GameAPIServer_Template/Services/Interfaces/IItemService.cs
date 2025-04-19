using GameAPIServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces;

public interface IItemService
{
    public Task<(ErrorCode, List<RewardData>)> ReceiveOneGacha(int uid, int gachaKey);

    public Task<ErrorCode> ReceiveReward(int uid, RewardData reward);
  

}
