using CSDL7.MasterService.Entities.Districts;
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
using CSDL7.MasterService.Entities.Wards;

namespace CSDL7.MasterService.Data.Wards
{
    public class EfCoreWardRepository : EfCoreRepository<MasterServiceDbContext, Ward, Guid>, IWardRepository
    {
        public EfCoreWardRepository(IDbContextProvider<MasterServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<WardWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(ward => new WardWithNavigationProperties
                {
                    Ward = ward,
                    District = dbContext.Set<District>().FirstOrDefault(c => c.Id == ward.DistrictId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<WardWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? name = null,
            string? code = null,
            Guid? districtId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, name, code, districtId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? WardConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<WardWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from ward in (await GetDbSetAsync())
                   join district in (await GetDbContextAsync()).Set<District>() on ward.DistrictId equals district.Id into districts
                   from district in districts.DefaultIfEmpty()
                   select new WardWithNavigationProperties
                   {
                       Ward = ward,
                       District = district
                   };
        }

        protected virtual IQueryable<WardWithNavigationProperties> ApplyFilter(
            IQueryable<WardWithNavigationProperties> query,
            string? filterText,
            string? name = null,
            string? code = null,
            Guid? districtId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Ward.Name!.Contains(filterText!) || e.Ward.Code!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Ward.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Ward.Code.Contains(code))
                    .WhereIf(districtId != null && districtId != Guid.Empty, e => e.District != null && e.District.Id == districtId);
        }

        public virtual async Task<List<Ward>> GetListAsync(
            string? filterText = null,
            string? name = null,
            string? code = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, name, code);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? WardConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? code = null,
            Guid? districtId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, name, code, districtId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Ward> ApplyFilter(
            IQueryable<Ward> query,
            string? filterText = null,
            string? name = null,
            string? code = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.Contains(filterText!) || e.Code!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code));
        }
    }
}