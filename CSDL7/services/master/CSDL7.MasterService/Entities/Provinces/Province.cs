using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace CSDL7.MasterService.Entities.Provinces
{
    public class Province : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string? ProvinceCode { get; set; }

        protected Province()
        {

        }

        public Province(Guid id, string name, string? provinceCode = null)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            ProvinceCode = provinceCode;
        }

    }
}