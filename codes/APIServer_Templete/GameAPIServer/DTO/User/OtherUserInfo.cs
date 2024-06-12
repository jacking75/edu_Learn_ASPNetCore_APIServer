using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.User
{
    public class OtherUserInfoRequest
    {
        [Required]
        public int Uid { get; set; }
    }

    public class OtherUserInfoResponse : ErrorCodeDTO
    {
        public OtherUserInfo UserInfo { get; set; }
    }

    public class OtherUserInfo
    {
        public int uid { get; set; }
        public string nickname { get; set; }
        public int total_bestscore { get; set; }
        public int total_bestscore_cur_season { get; set; }
        public int total_bestscore_prev_season { get; set; }
        public int main_char_key { get; set; }
        public int main_char_skin_key { get; set; }
        public string main_char_costume_json { get; set; }
        public Int64 rank;
    }
}
