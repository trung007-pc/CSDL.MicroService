using CSDL7.MasterService.Services.Dtos.Provinces;
using CSDL7.MasterService.Entities.Provinces;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace CSDL7.MasterService.Services.Dtos.Districts
{
    public class DistrictWithNavigationPropertiesDto
    {
        public DistrictDto District { get; set; } = null!;

        public ProvinceDto Province { get; set; } = null!;

    }
}