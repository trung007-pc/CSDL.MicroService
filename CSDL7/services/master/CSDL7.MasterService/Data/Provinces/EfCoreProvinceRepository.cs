using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using CSDL7.MasterService.Data;
using CSDL7.MasterService.Entities.Provinces;

namespace CSDL7.MasterService.Data.Provinces
{
    public class EfCoreProvinceRepository : EfCoreRepository<MasterServiceDbContext, Province, Guid>, IProvinceRepository
    {
        public EfCoreProvinceRepository(IDbContextProvider<MasterServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<List<Province>> GetListAsync(
            string? filterText = null,
            string? name = null,
            string? provinceCode = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, name, provinceCode);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProvinceConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? provinceCode = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, name, provinceCode);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Province> ApplyFilter(
            IQueryable<Province> query,
            string? filterText = null,
            string? name = null,
            string? provinceCode = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.Contains(filterText!) || e.ProvinceCode!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(provinceCode), e => e.ProvinceCode.Contains(provinceCode));
        }
    }
}