using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CSDL7.MasterService.Services.Dtos.Provinces
{
    public class ProvinceCreateDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? ProvinceCode { get; set; }
    }
}