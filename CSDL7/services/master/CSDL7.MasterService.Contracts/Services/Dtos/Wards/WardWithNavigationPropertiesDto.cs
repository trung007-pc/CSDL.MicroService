using CSDL7.MasterService.Services.Dtos.Districts;
using CSDL7.MasterService.Entities.Districts;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace CSDL7.MasterService.Services.Dtos.Wards
{
    public class WardWithNavigationPropertiesDto
    {
        public WardDto Ward { get; set; } = null!;

        public DistrictDto District { get; set; } = null!;

    }
}