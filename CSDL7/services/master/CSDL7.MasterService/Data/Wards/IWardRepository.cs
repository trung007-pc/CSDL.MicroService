using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using CSDL7.MasterService.Entities.Wards;

namespace CSDL7.MasterService.Data.Wards
{
    public interface IWardRepository : IRepository<Ward, Guid>
    {
        Task<WardWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<WardWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? name = null,
            string? code = null,
            Guid? districtId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Ward>> GetListAsync(
                    string? filterText = null,
                    string? name = null,
                    string? code = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? code = null,
            Guid? districtId = null,
            CancellationToken cancellationToken = default);
    }
}