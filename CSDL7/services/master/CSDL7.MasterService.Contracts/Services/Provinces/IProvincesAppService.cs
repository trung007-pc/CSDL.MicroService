using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using CSDL7.MasterService.Entities.Provinces;
using CSDL7.MasterService.Services.Dtos.Provinces;

namespace CSDL7.MasterService.Services.Provinces
{
    public interface IProvincesAppService : IApplicationService
    {

        Task<PagedResultDto<ProvinceDto>> GetListAsync(GetProvincesInput input);

        Task<ProvinceDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ProvinceDto> CreateAsync(ProvinceCreateDto input);

        Task<ProvinceDto> UpdateAsync(Guid id, ProvinceUpdateDto input);
    }
}