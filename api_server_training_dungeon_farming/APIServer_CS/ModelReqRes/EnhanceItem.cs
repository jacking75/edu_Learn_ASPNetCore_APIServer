using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelReqRes;

public class EnhanceItemRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    [Required]
    [Range(1, Int64.MaxValue, ErrorMessage = "INVENTORY_ITEM_ID IS NOT VALID")]
    public Int64 InventoryItemId { get; set; }
}

public class EnhanceItemResponse : ResponseBase
{
    public bool IsSuccess { get; set; }
}