using APIServer.Models;
using APIServer.Models.GameDB;
using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Item
{
    public class CharacterSetCostumeRequest
    {
        [Required]
        public int CharKey { get; set; }
        [Required]
        public CharCostumeInfo CostumeInfo { get; set; }
    }

    public class CharacterSetCostumeResponse : ErrorCodeDTO
    {
    }
}
