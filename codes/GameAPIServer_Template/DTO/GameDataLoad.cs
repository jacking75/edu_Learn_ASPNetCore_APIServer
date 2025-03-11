using GameAPIServer.DAO;
using System.Collections.Generic;

namespace GameAPIServer.DTO
{
    public class GameDataLoadResponse : ErrorCodeDTO
    {
        public DataLoadGameInfo GameData { get; set; }
    }

    public class DataLoadGameInfo
    {        
    }

    public class UserCharInfo
    {
        public GdbUserCharInfo CharInfo { get; set; }
      
    }
}
