using CSDL7.MasterService.Services.Dtos.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using CSDL7.MasterService.Entities.Districts;
using CSDL7.MasterService.Services.Dtos.Districts;

namespace CSDL7.MasterService.Services.Districts
{
    public interface IDistrictsAppService : IApplicationService
    {

        Task<PagedResultDto<DistrictWithNavigationPropertiesDto>> GetListAsync(GetDistrictsInput input);

        Task<DistrictWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<DistrictDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetProvinceLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<DistrictDto> CreateAsync(DistrictCreateDto input);

        Task<DistrictDto> UpdateAsync(Guid id, DistrictUpdateDto input);
    }
}