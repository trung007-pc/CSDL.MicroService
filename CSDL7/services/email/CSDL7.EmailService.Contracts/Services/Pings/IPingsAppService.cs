using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using CSDL7.EmailService.Entities.Pings;
using CSDL7.EmailService.Services.Dtos.Pings;

namespace CSDL7.EmailService.Services.Pings
{
    public interface IPingsAppService : IApplicationService
    {

        Task<PagedResultDto<PingDto>> GetListAsync(GetPingsInput input);

        Task<PingDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<PingDto> CreateAsync(PingCreateDto input);

        Task<PingDto> UpdateAsync(Guid id, PingUpdateDto input);
    }
}