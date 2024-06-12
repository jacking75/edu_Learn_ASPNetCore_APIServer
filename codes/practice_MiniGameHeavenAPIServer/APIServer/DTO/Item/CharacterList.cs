using APIServer.DTO.DataLoad;
using System.Collections.Generic;

namespace APIServer.DTO.Item
{
    public class CharacterListResponse : ErrorCodeDTO
    {
        public List<UserCharInfo> CharList { get; set; }
    }
}
