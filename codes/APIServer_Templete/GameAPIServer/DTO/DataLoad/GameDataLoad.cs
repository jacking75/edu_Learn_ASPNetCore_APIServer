using APIServer.Models.GameDB;
using System.Collections.Generic;

namespace APIServer.DTO.DataLoad
{
    public class GameDataLoadResponse : ErrorCodeDTO
    {
        public DataLoadGameInfo GameData { get; set; }
    }

    public class DataLoadGameInfo
    {
        public IEnumerable<GdbMiniGameInfo> GameList { get; set; }
        public List<UserCharInfo> CharList { get; set; }
        public IEnumerable<GdbUserSkinInfo> SkinList { get; set; }
        public IEnumerable<GdbUserCostumeInfo> CostumeList { get; set; }
        public IEnumerable<GdbUserFoodInfo> FoodList { get; set; }
    }

    public class UserCharInfo
    {
        public GdbUserCharInfo CharInfo { get; set; }
        public IEnumerable<GdbUserCharRandomSkillInfo> RandomSkills { get; set; }
    }
}
