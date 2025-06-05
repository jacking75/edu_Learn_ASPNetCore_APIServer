namespace GameServer.Models.DTO;

public class DataLoadResponse : ErrorCodeDTO
{
    public List<AssetInfo> CurrencyList { get; set; }
    public List<ItemInfo> ItemInfoList { get; set; }
    public List<Deck> DeckList{ get; set; }
    public List<MailInfo> MailList { get; set; }
    public List<AttendanceInfo> AttendanaceList { get; set; }
}

public class TableLoadReponse : ErrorCodeDTO
{
    public List<MdbAbilityInfo> ItemAbilityInfoList { get; set; }
    public List<MdbItemInfo> ItemInfoList { get; set; }
    
    //... 외에는 더 필요해지면 
}