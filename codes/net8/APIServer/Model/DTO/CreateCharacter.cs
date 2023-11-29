//TODO 사용하지 않으면 삭제할 것
using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO;

public class CreateCharacterReq
{
    [Required] public string Email { get; set; }

    [Required] public string AuthToken { get; set; }

    public string NickName { get; set; }
}

public class CreateCharacterRes
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
}

public class CreateCharacterInfo
{
    [Required]
    [MinLength(1, ErrorMessage = "NICKNAME CANNOT BE EMPTY")]
    [StringLength(8, ErrorMessage = "NICKNAME CANNOT EXCEED 18 CHARACTERS")]
    public string Nickname { get; set; }

    public int Eye { get; set; }

    public int HairStyle { get; set; }

    public int Mustache { get; set; }

    public int Cloak { get; set; }
    public int Pants { get; set; }
    public int Dress { get; set; }
    public int Armor { get; set; }
    public int Helmet { get; set; }
}