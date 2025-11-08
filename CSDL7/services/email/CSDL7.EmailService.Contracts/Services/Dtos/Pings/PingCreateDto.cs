using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CSDL7.EmailService.Services.Dtos.Pings
{
    public class PingCreateDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public int Value { get; set; }
    }
}