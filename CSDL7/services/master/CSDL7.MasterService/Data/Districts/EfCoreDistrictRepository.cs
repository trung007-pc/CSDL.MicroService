using CSDL7.MasterService.Entities.Provinces;
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
using CSDL7.MasterService.Entities.Districts;

namespace CSDL7.MasterService.Data.Districts
{
    public class EfCoreDistrictRepository : EfCoreRepository<MasterServiceDbContext, District, Guid>, IDistrictRepository
    {
        public EfCoreDistrictRepository(IDbContextProvider<MasterServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<DistrictWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(district => new DistrictWithNavigationProperties
                {
                    District = district,
                    Province = dbContext.Set<Province>().FirstOrDefault(c => c.Id == district.ProvinceId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<DistrictWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? name = null,
            string? code = null,
            Guid? provinceId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, name, code, provinceId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DistrictConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<DistrictWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from district in (await GetDbSetAsync())
                   join province in (await GetDbContextAsync()).Set<Province>() on district.ProvinceId equals province.Id into provinces
                   from province in provinces.DefaultIfEmpty()
                   select new DistrictWithNavigationProperties
                   {
                       District = district,
                       Province = province
                   };
        }

        protected virtual IQueryable<DistrictWithNavigationProperties> ApplyFilter(
            IQueryable<DistrictWithNavigationProperties> query,
            string? filterText,
            string? name = null,
            string? code = null,
            Guid? provinceId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.District.Name!.Contains(filterText!) || e.District.Code!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.District.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.District.Code.Contains(code))
                    .WhereIf(provinceId != null && provinceId != Guid.Empty, e => e.Province != null && e.Province.Id == provinceId);
        }

        public virtual async Task<List<District>> GetListAsync(
            string? filterText = null,
            string? name = null,
            string? code = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, name, code);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DistrictConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? code = null,
            Guid? provinceId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, name, code, provinceId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<District> ApplyFilter(
            IQueryable<District> query,
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