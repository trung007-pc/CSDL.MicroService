using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CSDL7.MasterService.Services.Dtos.Wards
{
    public class WardCreateDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public Guid DistrictId { get; set; }
    }
}