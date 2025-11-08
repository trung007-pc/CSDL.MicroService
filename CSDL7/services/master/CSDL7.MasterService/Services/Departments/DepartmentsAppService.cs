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
using CSDL7.MasterService.Services.Departments;

using CSDL7.MasterService.Entities.Departments;
using CSDL7.MasterService.Services.Dtos.Departments;
using CSDL7.MasterService.Data.Departments;
using CSDL7.MasterService.Entities.Commons;
using CSDL7.MasterService.Services.Dtos.Shared;
using CSDL7.Shared;
using CSDL7.Shared.Enums;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EventBus.Distributed;

namespace CSDL7.MasterService.Services.Departments
{
    /// <summary>
    /// Application service for managing Department CRUD operations.
    /// Handles authorization, DTO mapping, and orchestration of domain services.
    /// </summary>
    [Authorize(MasterServicePermissions.Departments.Default)]
    public class DepartmentsAppService : ApplicationService, IDepartmentsAppService
    {
        private readonly DepartmentManager _departmentManager;
        private readonly IDistributedEventBus _distributedEventBus;
        
        /// <summary>
        /// Constructor - Injects DepartmentManager for business logic handling
        /// </summary>
        /// <param name="departmentRepository">Repository interface (not used directly)</param>
        /// <param name="departmentManager">Domain service containing Department business logic</param>
        public DepartmentsAppService(IDepartmentRepository departmentRepository, DepartmentManager departmentManager, IDistributedEventBus distributedEventBus)
        {
            _departmentManager = departmentManager;                                                                                                                                                                                                                                                                     
            _distributedEventBus = distributedEventBus;
        }

        /// <summary>
        /// Gets paginated list of Departments with filtering and sorting capabilities.
        /// Requires permission: MasterService.Departments (Default - Read)
        /// </summary>
        /// <param name="input">Input parameters including: FilterText, Name, Sorting, SkipCount, MaxResultCount</param>
        /// <returns>Paginated list of Departments with total count</returns>
        public virtual async Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentsInput input)
        {
            var result = await _departmentManager.GetListAsync(input);
            return ObjectMapper.Map<PagedData<Department>, PagedResultDto<DepartmentDto>>(result);
        }

        /// <summary>
        /// Gets a single Department by ID.
        /// Requires permission: MasterService.Departments (Default - Read)
        /// </summary>
        /// <param name="id">The ID of the Department to retrieve</param>
        /// <returns>Department details including audit fields</returns>
        /// <exception cref="EntityNotFoundException">Thrown when Department with given ID is not found</exception>
        public virtual async Task<DepartmentDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Department, DepartmentDto>(await _departmentManager.GetAsync(id));
        }

        /// <summary>
        /// Deletes a Department (soft delete).
        /// Requires permission: MasterService.Departments.Delete
        /// </summary>
        /// <param name="id">The ID of the Department to delete</param>
        /// <returns>Task representing the asynchronous operation</returns>
        /// <exception cref="EntityNotFoundException">Thrown when Department with given ID is not found</exception>
        [Authorize(MasterServicePermissions.Departments.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _departmentManager.DeleteAsync(id);
        }

        /// <summary>
        /// Creates a new Department.
        /// Requires permission: MasterService.Departments.Create
        /// </summary>
        /// <param name="input">Department creation data (Name is required)</param>
        /// <returns>The newly created Department with full details (ID, CreationTime, etc.)</returns>
        /// <exception cref="BusinessException">Thrown when validation fails (e.g., empty name or duplicate)</exception>
        [Authorize(MasterServicePermissions.Departments.Create)]
        public virtual async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
        {
            var department = await _departmentManager.CreateAsync(
            input.Name
            );
            
            await _distributedEventBus.PublishAsync(new DepartmentCreatedEto()
            {
                Id = department.Id,
                Name = department.Name,
                CreatedAt = department.CreationTime 
            });
            
            return ObjectMapper.Map<Department, DepartmentDto>(department);
        }

        /// <summary>
        /// Updates an existing Department.
        /// Requires permission: MasterService.Departments.Edit
        /// Uses ConcurrencyStamp to prevent lost updates (optimistic concurrency).
        /// </summary>
        /// <param name="id">The ID of the Department to update</param>
        /// <param name="input">Updated Department data (Name and ConcurrencyStamp)</param>
        /// <returns>The updated Department</returns>
        /// <exception cref="EntityNotFoundException">Thrown when Department with given ID is not found</exception>
        /// <exception cref="DbUpdateConcurrencyException">Thrown when ConcurrencyStamp doesn't match (data was modified by another user)</exception>
        [Authorize(MasterServicePermissions.Departments.Edit)]
        public virtual async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input)
        {
            var department = await _departmentManager.UpdateAsync(
            id,
            input.Name, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Department, DepartmentDto>(department);
        }

        public async Task CallPingEmailAsync()
        {
            
             // await _distributedEventBus.PublishAsync(new PingCalledEto()
             //                {
             //                   Value = 12
             //                });
             for (int i = 1; i <= 1000; i++)
                                           {
                                               await _distributedEventBus.PublishAsync(new PingCalledEto()
                                               {
                                                  Value = i
                                               });
                                           }
  
        }
    }
}