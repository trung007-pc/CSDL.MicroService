using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;
using CSDL7.MasterService.Data.Districts;

namespace CSDL7.MasterService.Entities.Districts
{
    public class DistrictManager : DomainService
    {
        protected IDistrictRepository _districtRepository;

        public DistrictManager(IDistrictRepository districtRepository)
        {
            _districtRepository = districtRepository;
        }

        public virtual async Task<District> CreateAsync(
        Guid provinceId, string name, string? code = null)
        {
            Check.NotNull(provinceId, nameof(provinceId));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var district = new District(
             GuidGenerator.Create(),
             provinceId, name, code
             );

            return await _districtRepository.InsertAsync(district);
        }

        public virtual async Task<District> UpdateAsync(
            Guid id,
            Guid provinceId, string name, string? code = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(provinceId, nameof(provinceId));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var district = await _districtRepository.GetAsync(id);

            district.ProvinceId = provinceId;
            district.Name = name;
            district.Code = code;

            district.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _districtRepository.UpdateAsync(district);
        }

    }
}