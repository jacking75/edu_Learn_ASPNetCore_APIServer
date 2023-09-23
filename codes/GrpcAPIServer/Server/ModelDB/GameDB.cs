namespace ModelGameDB;

public class EnterFieldCharacterInfo
{
    public Int64 AccountId { get; set; }
    public Int64 CharacterId { get; set; }
    public Int16 ChannelNum { get; set; }
}


public class CharacterInfo
{
    public Int32 Level { get; set; }
    public string NickName { get; set; } = "";
}