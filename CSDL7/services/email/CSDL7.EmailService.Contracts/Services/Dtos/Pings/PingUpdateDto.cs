using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace CSDL7.EmailService.Services.Dtos.Pings
{
    public class PingUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string Name { get; set; } = null!;
        public int Value { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}