using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CSDL7.MasterService.Services.Dtos.Districts
{
    public class DistrictCreateDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public Guid ProvinceId { get; set; }
    }
}