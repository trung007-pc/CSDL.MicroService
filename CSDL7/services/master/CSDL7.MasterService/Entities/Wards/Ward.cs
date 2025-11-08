using CSDL7.MasterService.Entities.Districts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace CSDL7.MasterService.Entities.Wards
{
    public class Ward : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string? Code { get; set; }
        public Guid DistrictId { get; set; }

        protected Ward()
        {

        }

        public Ward(Guid id, Guid districtId, string name, string? code = null)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            Code = code;
            DistrictId = districtId;
        }

    }
}