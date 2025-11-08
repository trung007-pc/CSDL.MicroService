using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace CSDL7.MasterService.Services.Dtos.Districts
{
    public class DistrictUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public Guid ProvinceId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}