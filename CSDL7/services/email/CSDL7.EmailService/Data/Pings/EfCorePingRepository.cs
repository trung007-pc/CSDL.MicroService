using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using CSDL7.EmailService.Data;
using CSDL7.EmailService.Entities.Pings;

namespace CSDL7.EmailService.Data.Pings
{
    public class EfCorePingRepository : EfCoreRepository<EmailServiceDbContext, Ping, Guid>, IPingRepository
    {
        public EfCorePingRepository(IDbContextProvider<EmailServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<List<Ping>> GetListAsync(
            string? filterText = null,
            string? name = null,
            int? valueMin = null,
            int? valueMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, name, valueMin, valueMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PingConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            int? valueMin = null,
            int? valueMax = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, name, valueMin, valueMax);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Ping> ApplyFilter(
            IQueryable<Ping> query,
            string? filterText = null,
            string? name = null,
            int? valueMin = null,
            int? valueMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(valueMin.HasValue, e => e.Value >= valueMin!.Value)
                    .WhereIf(valueMax.HasValue, e => e.Value <= valueMax!.Value);
        }
    }
}