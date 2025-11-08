using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace CSDL7.EmailService.Services.Dtos.Pings
{
    public class PingDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;
        public int Value { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}