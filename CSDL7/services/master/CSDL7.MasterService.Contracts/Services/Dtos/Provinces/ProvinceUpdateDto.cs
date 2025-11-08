using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace CSDL7.MasterService.Services.Dtos.Provinces
{
    public class ProvinceUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? ProvinceCode { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}