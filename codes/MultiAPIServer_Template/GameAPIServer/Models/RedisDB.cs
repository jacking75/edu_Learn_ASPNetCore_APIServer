namespace GameAPIServer.Models;

//RedisDB의 객체는 객체 이름 앞에 Rdb를 붙인다.

public class RdbAuthUserData
{
    public int Uid { get; set; } = 0;
    public string Token { get; set; } = "";
}

public class RdbUserScoreData
{
    public int uid { get; set; } = 0;
    public int total_bestscore { get; set; } = 0;
}
