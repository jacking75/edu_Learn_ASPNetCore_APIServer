//TODO 사용하지 않으면 삭제할 것
using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO;

public class EnterFieldReq
{
    [Required] public string Email { get; set; }
    [Required] public string AuthToken { get; set; }
}

public class EnterFieldRes
{
    [Required] public ErrorCode Result { get; set; }
    [Required] public string EnterFieldToken { get; set; }
    [Required] public string WorldAddressIp { get; set; }
    [Required] public string WorldAddressPort { get; set; }
}