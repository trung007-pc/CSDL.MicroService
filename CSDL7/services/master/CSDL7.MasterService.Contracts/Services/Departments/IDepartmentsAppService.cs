using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using CSDL7.MasterService.Entities.Departments;
using CSDL7.MasterService.Services.Dtos.Departments;

namespace CSDL7.MasterService.Services.Departments
{
    public interface IDepartmentsAppService : IApplicationService
    {

        Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentsInput input);

        Task<DepartmentDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<DepartmentDto> CreateAsync(CreateDepartmentDto input);

        Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input);

        Task CallPingEmailAsync();
    }
}