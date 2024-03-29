﻿using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class LoginRequest
    {
        [StringLength(45)]
        public string ID { get; set; } = "";
        [StringLength(200)]
        public string PW { get; set; } = "";
    }
}