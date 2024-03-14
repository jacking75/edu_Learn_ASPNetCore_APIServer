using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiServer.Model
{
    public class TableLoginData
    {
        [StringLength(200)]
        public string PW { get; set; } = "";
        [StringLength(200)]
        public string Salt { get; set; } = "";
    }
}