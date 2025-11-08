using CSDL7.MasterService.Services.Dtos.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using CSDL7.MasterService.Entities.Wards;
using CSDL7.MasterService.Services.Dtos.Wards;

namespace CSDL7.MasterService.Services.Wards
{
    public interface IWardsAppService : IApplicationService
    {

        Task<PagedResultDto<WardWithNavigationPropertiesDto>> GetListAsync(GetWardsInput input);

        Task<WardWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<WardDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetDistrictLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<WardDto> CreateAsync(WardCreateDto input);

        Task<WardDto> UpdateAsync(Guid id, WardUpdateDto input);
    }
}