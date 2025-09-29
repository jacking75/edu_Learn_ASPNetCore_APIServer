
namespace GameAPIServer.Models.DTO;

public class GameDataLoadResponse : ErrorCode
{
    public DataLoadGameInfo GameData { get; set; }
}

public class DataLoadGameInfo
{        
}

public class UserCharInfo
{
    public DAO.GdbUserCharInfo CharInfo { get; set; }
  
}
