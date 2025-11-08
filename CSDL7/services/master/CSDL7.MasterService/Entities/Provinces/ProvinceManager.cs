using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;
using CSDL7.MasterService.Data.Provinces;

namespace CSDL7.MasterService.Entities.Provinces
{
    public class ProvinceManager : DomainService
    {
        protected IProvinceRepository _provinceRepository;

        public ProvinceManager(IProvinceRepository provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        public virtual async Task<Province> CreateAsync(
        string name, string? provinceCode = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var province = new Province(
             GuidGenerator.Create(),
             name, provinceCode
             );

            return await _provinceRepository.InsertAsync(province);
        }

        public virtual async Task<Province> UpdateAsync(
            Guid id,
            string name, string? provinceCode = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var province = await _provinceRepository.GetAsync(id);

            province.Name = name;
            province.ProvinceCode = provinceCode;

            province.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _provinceRepository.UpdateAsync(province);
        }

    }
}