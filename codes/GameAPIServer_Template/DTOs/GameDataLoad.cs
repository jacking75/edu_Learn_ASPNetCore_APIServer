using GameAPIServer.Models;

namespace GameAPIServer.DTOs;

public class GameDataLoadResponse : ErrorCode
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
