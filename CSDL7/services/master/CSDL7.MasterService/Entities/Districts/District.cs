using CSDL7.MasterService.Entities.Provinces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace CSDL7.MasterService.Entities.Districts
{
    public class District : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string? Code { get; set; }
        public Guid ProvinceId { get; set; }

        protected District()
        {

        }

        public District(Guid id, Guid provinceId, string name, string? code = null)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            Code = code;
            ProvinceId = provinceId;
        }

    }
}