
using System.ComponentModel.DataAnnotations;

namespace HearthStoneWeb.Models.Game;



public class ItemInfoListResponse : ErrorCodeDTO
{
    public List<ItemInfo> ItemList { get; set; }
}

public class ItemRandomRequest
{
    [Required]
    public int GachaId { get; set; }
}

public class ItemRandomResponse : ErrorCodeDTO
{
    public List<ItemInfo> ItemList { get; set; }
}

// 필요한 DTO 클래스들
public class SaveDeckRequest
{
    [Required]
    public int DeckId { get; set; }

    [Required]
    public string DeckList { get; set; }
}

public class SaveDeckResponse : ErrorCodeDTO
{
}

public class SetMainDeckRequest
{
    [Required]
    public int DeckId { get; set; }
}

public class SetMainDeckResponse : ErrorCodeDTO
{
}

public class DeckInfoListResponse : ErrorCodeDTO
{
    public List<DeckInfo> DeckList { get; set; } = new List<DeckInfo>();
}
