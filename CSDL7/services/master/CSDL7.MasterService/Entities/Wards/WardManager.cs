using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;
using CSDL7.MasterService.Data.Wards;

namespace CSDL7.MasterService.Entities.Wards
{
    public class WardManager : DomainService
    {
        protected IWardRepository _wardRepository;

        public WardManager(IWardRepository wardRepository)
        {
            _wardRepository = wardRepository;
        }

        public virtual async Task<Ward> CreateAsync(
        Guid districtId, string name, string? code = null)
        {
            Check.NotNull(districtId, nameof(districtId));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var ward = new Ward(
             GuidGenerator.Create(),
             districtId, name, code
             );

            return await _wardRepository.InsertAsync(ward);
        }

        public virtual async Task<Ward> UpdateAsync(
            Guid id,
            Guid districtId, string name, string? code = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(districtId, nameof(districtId));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var ward = await _wardRepository.GetAsync(id);

            ward.DistrictId = districtId;
            ward.Name = name;
            ward.Code = code;

            ward.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _wardRepository.UpdateAsync(ward);
        }

    }
}