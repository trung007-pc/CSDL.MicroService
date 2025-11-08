using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace CSDL7.MasterService.Services.Dtos.Departments
{
    public class UpdateDepartmentDto : IHasConcurrencyStamp
    {
        [Required]
        public string Name { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;
    }
}