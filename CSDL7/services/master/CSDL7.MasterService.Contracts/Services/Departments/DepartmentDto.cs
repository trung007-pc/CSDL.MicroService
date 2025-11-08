using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace CSDL7.MasterService.Services.Dtos.Departments
{
    public class DepartmentDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;

    }
}