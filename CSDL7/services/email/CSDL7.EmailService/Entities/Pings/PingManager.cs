using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;
using CSDL7.EmailService.Data.Pings;

namespace CSDL7.EmailService.Entities.Pings
{
    public class PingManager : DomainService
    {
        protected IPingRepository _pingRepository;

        public PingManager(IPingRepository pingRepository)
        {
            _pingRepository = pingRepository;
        }

        public virtual async Task<Ping> CreateAsync(
        string name, int value)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var ping = new Ping(
             GuidGenerator.Create(),
             name, value
             );

            return await _pingRepository.InsertAsync(ping);
        }

        public virtual async Task<Ping> UpdateAsync(int value, [CanBeNull] string? concurrencyStamp = null
        )
        {
            var ping = await _pingRepository.FirstOrDefaultAsync();
            ping.Value = value;
            
            ping.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _pingRepository.UpdateAsync(ping);
        }

    }
}