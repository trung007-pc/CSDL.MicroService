using CSDL7.MasterService.Services.Dtos.Shared;
using CSDL7.MasterService.Entities.Provinces;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using CSDL7.MasterService.Permissions;
using CSDL7.MasterService.Services.Districts;

using CSDL7.MasterService.Entities.Districts;
using CSDL7.MasterService.Services.Dtos.Districts;
using CSDL7.MasterService.Data.Districts;
using CSDL7.MasterService.Services.Dtos.Shared;

namespace CSDL7.MasterService.Services.Districts
{

    [Authorize(MasterServicePermissions.Districts.Default)]
    public class DistrictsAppService : ApplicationService, IDistrictsAppService
    {

        protected IDistrictRepository _districtRepository;
        protected DistrictManager _districtManager;

        protected IRepository<CSDL7.MasterService.Entities.Provinces.Province, Guid> _provinceRepository;

        public DistrictsAppService(IDistrictRepository districtRepository, DistrictManager districtManager, IRepository<CSDL7.MasterService.Entities.Provinces.Province, Guid> provinceRepository)
        {

            _districtRepository = districtRepository;
            _districtManager = districtManager; _provinceRepository = provinceRepository;

        }

        public virtual async Task<PagedResultDto<DistrictWithNavigationPropertiesDto>> GetListAsync(GetDistrictsInput input)
        {
            var totalCount = await _districtRepository.GetCountAsync(input.FilterText, input.Name, input.Code, input.ProvinceId);
            var items = await _districtRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Name, input.Code, input.ProvinceId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<DistrictWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<DistrictWithNavigationProperties>, List<DistrictWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<DistrictWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<DistrictWithNavigationProperties, DistrictWithNavigationPropertiesDto>
                (await _districtRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<DistrictDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<District, DistrictDto>(await _districtRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetProvinceLookupAsync(LookupRequestDto input)
        {
            var query = (await _provinceRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<CSDL7.MasterService.Entities.Provinces.Province>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CSDL7.MasterService.Entities.Provinces.Province>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(MasterServicePermissions.Districts.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _districtRepository.DeleteAsync(id);
        }

        [Authorize(MasterServicePermissions.Districts.Create)]
        public virtual async Task<DistrictDto> CreateAsync(DistrictCreateDto input)
        {
            if (input.ProvinceId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Province"]]);
            }

            var district = await _districtManager.CreateAsync(
            input.ProvinceId, input.Name, input.Code
            );

            return ObjectMapper.Map<District, DistrictDto>(district);
        }

        [Authorize(MasterServicePermissions.Districts.Edit)]
        public virtual async Task<DistrictDto> UpdateAsync(Guid id, DistrictUpdateDto input)
        {
            if (input.ProvinceId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Province"]]);
            }

            var district = await _districtManager.UpdateAsync(
            id,
            input.ProvinceId, input.Name, input.Code, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<District, DistrictDto>(district);
        }
    }
}