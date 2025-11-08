using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace CSDL7.MasterService.Entities.Departments
{
    public class Department : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        protected Department()
        {

        }

        public Department(Guid id, string name)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
        }

    }
}