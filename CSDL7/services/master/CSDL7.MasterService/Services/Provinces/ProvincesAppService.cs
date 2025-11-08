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
using CSDL7.MasterService.Services.Provinces;

using CSDL7.MasterService.Entities.Provinces;
using CSDL7.MasterService.Services.Dtos.Provinces;
using CSDL7.MasterService.Data.Provinces;
using CSDL7.MasterService.Services.Dtos.Shared;

namespace CSDL7.MasterService.Services.Provinces
{

    [Authorize(MasterServicePermissions.Provinces.Default)]
    public class ProvincesAppService : ApplicationService, IProvincesAppService
    {

        protected IProvinceRepository _provinceRepository;
        protected ProvinceManager _provinceManager;

        public ProvincesAppService(IProvinceRepository provinceRepository, ProvinceManager provinceManager)
        {

            _provinceRepository = provinceRepository;
            _provinceManager = provinceManager;

        }

        public virtual async Task<PagedResultDto<ProvinceDto>> GetListAsync(GetProvincesInput input)
        {
            var totalCount = await _provinceRepository.GetCountAsync(input.FilterText, input.Name, input.ProvinceCode);
            var items = await _provinceRepository.GetListAsync(input.FilterText, input.Name, input.ProvinceCode, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ProvinceDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Province>, List<ProvinceDto>>(items)
            };
        }

        public virtual async Task<ProvinceDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Province, ProvinceDto>(await _provinceRepository.GetAsync(id));
        }

        [Authorize(MasterServicePermissions.Provinces.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _provinceRepository.DeleteAsync(id);
        }

        [Authorize(MasterServicePermissions.Provinces.Create)]
        public virtual async Task<ProvinceDto> CreateAsync(ProvinceCreateDto input)
        {

            var province = await _provinceManager.CreateAsync(
            input.Name, input.ProvinceCode
            );

            return ObjectMapper.Map<Province, ProvinceDto>(province);
        }

        [Authorize(MasterServicePermissions.Provinces.Edit)]
        public virtual async Task<ProvinceDto> UpdateAsync(Guid id, ProvinceUpdateDto input)
        {

            var province = await _provinceManager.UpdateAsync(
            id,
            input.Name, input.ProvinceCode, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Province, ProvinceDto>(province);
        }
    }
}