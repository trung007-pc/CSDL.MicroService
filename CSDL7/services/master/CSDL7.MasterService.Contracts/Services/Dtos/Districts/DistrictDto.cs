using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace CSDL7.MasterService.Services.Dtos.Districts
{
    public class DistrictDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public Guid ProvinceId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}