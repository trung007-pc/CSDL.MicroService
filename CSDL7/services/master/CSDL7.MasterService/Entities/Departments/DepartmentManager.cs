using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper.Internal.Mappers;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;
using CSDL7.MasterService.Data.Departments;
using CSDL7.MasterService.Entities.Commons;
using CSDL7.MasterService.Services.Dtos.Departments;
using TN3.Tenant.Admin.Domain.Commons;
using Volo.Abp.Application.Dtos;

namespace CSDL7.MasterService.Entities.Departments
{
    /// <summary>
    /// Domain service for managing Department business logic.
    /// Handles entity creation, validation, and coordination with repository.
    /// This service should contain ONLY pure business logic, NO infrastructure concerns (logging, caching, etc.)
    /// </summary>
    public class DepartmentManager : DomainService
    {
        protected IDepartmentRepository _departmentRepository;

        /// <summary>
        /// Constructor - Injects Department repository for data access
        /// </summary>
        /// <param name="departmentRepository">Repository for Department entity operations</param>
        public DepartmentManager(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        /// <summary>
        /// Creates a new Department entity with business validation.
        /// Generates a sequential GUID for the ID and validates the name.
        /// </summary>
        /// <param name="name">Department name (required, cannot be null or whitespace)</param>
        /// <returns>The newly created Department entity</returns>
        /// <exception cref="ArgumentException">Thrown when name is null or whitespace</exception>
        public virtual async Task<Department> CreateAsync(
        string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var department = new Department(
             GuidGenerator.Create(),
             name
             );

            return await _departmentRepository.InsertAsync(department);
        }

        /// <summary>
        /// Updates an existing Department entity.
        /// Validates the new name and handles optimistic concurrency control.
        /// </summary>
        /// <param name="id">The ID of the Department to update</param>
        /// <param name="name">New department name (required, cannot be null or whitespace)</param>
        /// <param name="concurrencyStamp">Concurrency stamp for optimistic locking (optional)</param>
        /// <returns>The updated Department entity</returns>
        /// <exception cref="ArgumentException">Thrown when name is null or whitespace</exception>
        /// <exception cref="EntityNotFoundException">Thrown when Department with given ID is not found</exception>
        /// <exception cref="DbUpdateConcurrencyException">Thrown when concurrency stamp doesn't match</exception>
        public virtual async Task<Department> UpdateAsync(
            Guid id,
            string name, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var department = await _departmentRepository.GetAsync(id);

            department.Name = name;

            department.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _departmentRepository.UpdateAsync(department);
        }
        
        /// <summary>
        /// Deletes a Department (soft delete - sets IsDeleted flag).
        /// </summary>
        /// <param name="id">The ID of the Department to delete</param>
        /// <returns>Task representing the asynchronous operation</returns>
        /// <exception cref="EntityNotFoundException">Thrown when Department with given ID is not found</exception>
        public virtual async Task DeleteAsync(Guid id)
        {
            await _departmentRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Retrieves a single Department by ID.
        /// </summary>
        /// <param name="id">The ID of the Department to retrieve</param>
        /// <returns>The Department entity</returns>
        /// <exception cref="EntityNotFoundException">Thrown when Department with given ID is not found</exception>
        public virtual async Task<Department> GetAsync(Guid id)
        {
            return (await _departmentRepository.GetAsync(id));
        }
        
        /// <summary>
        /// Retrieves a paginated list of Departments with filtering and sorting.
        /// Executes two separate queries: one for count and one for data retrieval.
        /// </summary>
        /// <param name="input">Query parameters including filters (FilterText, Name), sorting, and pagination (SkipCount, MaxResultCount)</param>
        /// <returns>Paginated result containing total count and list of Department entities</returns>
        public virtual async Task<PagedData<Department>> GetListAsync(GetDepartmentsInput input)
        {
            var totalCount = await _departmentRepository.GetCountAsync(input.FilterText, input.Name);
            var items = await _departmentRepository.GetListAsync(input.FilterText, input.Name, input.Sorting, input.MaxResultCount, input.SkipCount);
             
            return new PagedData<Department>()
            {
               TotalCount = totalCount,
               Items = items
            };
        }
    }
}