using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace CSDL7.EmailService.Entities.Pings
{
    public class Ping : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        public virtual int Value { get; set; }

        protected Ping()
        {

        }

        public Ping(Guid id, string name, int value)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            Value = value;
        }

    }
}