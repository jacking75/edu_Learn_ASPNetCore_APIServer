using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Game
{
    public class MiniGameSetPlayCharRequest
    {
        [Required]
        public int GameKey { get; set; }
        [Required]
        public int CharKey { get; set; }
    }

    public class MiniGameSetPlayCharResponse : ErrorCodeDTO
    {
    }
}
