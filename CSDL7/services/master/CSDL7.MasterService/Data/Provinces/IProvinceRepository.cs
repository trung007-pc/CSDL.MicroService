using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using CSDL7.MasterService.Entities.Provinces;

namespace CSDL7.MasterService.Data.Provinces
{
    public interface IProvinceRepository : IRepository<Province, Guid>
    {
        Task<List<Province>> GetListAsync(
            string? filterText = null,
            string? name = null,
            string? provinceCode = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? provinceCode = null,
            CancellationToken cancellationToken = default);
    }
}