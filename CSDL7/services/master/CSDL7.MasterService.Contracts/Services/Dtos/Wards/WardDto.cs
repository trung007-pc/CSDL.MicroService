using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace CSDL7.MasterService.Services.Dtos.Wards
{
    public class WardDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public Guid DistrictId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}