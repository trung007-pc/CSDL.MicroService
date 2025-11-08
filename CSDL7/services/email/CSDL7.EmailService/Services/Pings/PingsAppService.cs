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
using CSDL7.EmailService.Permissions;
using CSDL7.EmailService.Services.Pings;

using CSDL7.EmailService.Entities.Pings;
using CSDL7.EmailService.Services.Dtos.Pings;
using CSDL7.EmailService.Data.Pings;
using CSDL7.EmailService.Services.Dtos.Shared;

namespace CSDL7.EmailService.Services.Pings
{

    [Authorize(EmailServicePermissions.Pings.Default)]
    public class PingsAppService : ApplicationService, IPingsAppService
    {

        protected IPingRepository _pingRepository;
        protected PingManager _pingManager;

        public PingsAppService(IPingRepository pingRepository, PingManager pingManager)
        {

            _pingRepository = pingRepository;
            _pingManager = pingManager;

        }

        public virtual async Task<PagedResultDto<PingDto>> GetListAsync(GetPingsInput input)
        {
            var totalCount = await _pingRepository.GetCountAsync(input.FilterText, input.Name, input.ValueMin, input.ValueMax);
            var items = await _pingRepository.GetListAsync(input.FilterText, input.Name, input.ValueMin, input.ValueMax, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PingDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Ping>, List<PingDto>>(items)
            };
        }

        public virtual async Task<PingDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Ping, PingDto>(await _pingRepository.GetAsync(id));
        }

        [Authorize(EmailServicePermissions.Pings.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _pingRepository.DeleteAsync(id);
        }

        [Authorize(EmailServicePermissions.Pings.Create)]
        public virtual async Task<PingDto> CreateAsync(PingCreateDto input)
        {

            var ping = await _pingManager.CreateAsync(
            input.Name, input.Value
            );

            return ObjectMapper.Map<Ping, PingDto>(ping);
        }

        [Authorize(EmailServicePermissions.Pings.Edit)]
        public virtual async Task<PingDto> UpdateAsync(Guid id, PingUpdateDto input)
        {

            // var ping = await _pingManager.UpdateAsync(
            // id,
            // input.Name, input.Value, input.ConcurrencyStamp
            // );

            // return ObjectMapper.Map<Ping, PingDto>(ping);

            return null;
        }
    }
}