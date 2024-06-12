using APIServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Mail
{

    public class MailReceiveRequest
    {
        [Required]
        public int MailSeq { get; set; }
    }

    public class MailReceiveResponse : ErrorCodeDTO
    {
        public List<ReceivedReward> Rewards { get; set; }
    }

}
