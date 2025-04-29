using GameAPIServer.DTO.DataLoad;
using GameAPIServer.Models;
using GameAPIServer.Models.GameDB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces
{
    public interface IItemService
    {
        public Task<(ErrorCode, List<UserCharInfo>)> GetCharList(int uid);
        public Task<ErrorCode> ReceiveChar(int uid, int charKey, int qty);
        public Task<(ErrorCode, IEnumerable<GdbUserSkinInfo>)> GetSkinList(int uid);
        public Task<ErrorCode> ReceiveSkin(int uid, int skinKey);
        public Task<(ErrorCode, IEnumerable<GdbUserCostumeInfo>)> GetCostumeList(int uid);
        public Task<ErrorCode> ReceiveCostume(int uid, int costumeKey, int qty);
        public Task<(ErrorCode, IEnumerable<GdbUserFoodInfo>)> GetFoodList(int uid);
        public Task<ErrorCode> ReceiveFood(int uid, int foodKey, int qty);
        public Task<ErrorCode> ReceiveFoodGear(int uid, int foodKey, int gearQty);
        public Task<(ErrorCode, List<RewardData>)> ReceiveOneGacha(int uid, int gachaKey);
        public Task<ErrorCode> ReceiveReward(int uid, RewardData reward);
        public Task<ErrorCode> SetCharCostume(int uid, int charKey, CharCostumeInfo costumeInfo);

    }
}
