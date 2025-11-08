using CSDL7.MasterService.Services.Dtos.Shared;
using CSDL7.MasterService.Entities.Districts;
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
using CSDL7.MasterService.Services.Wards;

using CSDL7.MasterService.Entities.Wards;
using CSDL7.MasterService.Services.Dtos.Wards;
using CSDL7.MasterService.Data.Wards;
using CSDL7.MasterService.Services.Dtos.Shared;

namespace CSDL7.MasterService.Services.Wards
{

    [Authorize(MasterServicePermissions.Wards.Default)]
    public class WardsAppService : ApplicationService, IWardsAppService
    {

        protected IWardRepository _wardRepository;
        protected WardManager _wardManager;

        protected IRepository<CSDL7.MasterService.Entities.Districts.District, Guid> _districtRepository;

        public WardsAppService(IWardRepository wardRepository, WardManager wardManager, IRepository<CSDL7.MasterService.Entities.Districts.District, Guid> districtRepository)
        {

            _wardRepository = wardRepository;
            _wardManager = wardManager; _districtRepository = districtRepository;

        }

        public virtual async Task<PagedResultDto<WardWithNavigationPropertiesDto>> GetListAsync(GetWardsInput input)
        {
            var totalCount = await _wardRepository.GetCountAsync(input.FilterText, input.Name, input.Code, input.DistrictId);
            var items = await _wardRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Name, input.Code, input.DistrictId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<WardWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<WardWithNavigationProperties>, List<WardWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<WardWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<WardWithNavigationProperties, WardWithNavigationPropertiesDto>
                (await _wardRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<WardDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Ward, WardDto>(await _wardRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDistrictLookupAsync(LookupRequestDto input)
        {
            var query = (await _districtRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<CSDL7.MasterService.Entities.Districts.District>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CSDL7.MasterService.Entities.Districts.District>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(MasterServicePermissions.Wards.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _wardRepository.DeleteAsync(id);
        }

        [Authorize(MasterServicePermissions.Wards.Create)]
        public virtual async Task<WardDto> CreateAsync(WardCreateDto input)
        {
            if (input.DistrictId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["District"]]);
            }

            var ward = await _wardManager.CreateAsync(
            input.DistrictId, input.Name, input.Code
            );

            return ObjectMapper.Map<Ward, WardDto>(ward);
        }

        [Authorize(MasterServicePermissions.Wards.Edit)]
        public virtual async Task<WardDto> UpdateAsync(Guid id, WardUpdateDto input)
        {
            if (input.DistrictId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["District"]]);
            }

            var ward = await _wardManager.UpdateAsync(
            id,
            input.DistrictId, input.Name, input.Code, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Ward, WardDto>(ward);
        }
    }
}