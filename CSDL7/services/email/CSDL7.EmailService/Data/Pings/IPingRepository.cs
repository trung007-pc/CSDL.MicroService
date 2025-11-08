using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using CSDL7.EmailService.Entities.Pings;

namespace CSDL7.EmailService.Data.Pings
{
    public interface IPingRepository : IRepository<Ping, Guid>
    {
        Task<List<Ping>> GetListAsync(
            string? filterText = null,
            string? name = null,
            int? valueMin = null,
            int? valueMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            int? valueMin = null,
            int? valueMax = null,
            CancellationToken cancellationToken = default);
    }
}