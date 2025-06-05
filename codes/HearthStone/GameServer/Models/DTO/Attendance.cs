
using System.ComponentModel.DataAnnotations;

namespace GameServer.Models.DTO;

public class AttendanceInfoResponse : ErrorCodeDTO
{
    public List<AttendanceInfo> AttendanceInfoList { get; set; }
}

public class ReceivedReward 
{
   public List<AssetInfo> CurrencyList { get; set; }
   public List<ItemInfo> ItemList { get; set; }
}
public class AttendanceCheckRequest
{
    [Required]
    public int eventKey { get; set; }
}

public class AttendanceCheckResponse : ErrorCodeDTO
{
    public ReceivedReward? ReceivedReward { get; set; }
}