using CSDL7.MasterService.Entities.Provinces;

using System;
using System.Collections.Generic;
using CSDL7.MasterService.Entities.Districts;

namespace CSDL7.MasterService.Entities.Districts
{
    public  class DistrictWithNavigationProperties
    {
        public District District { get; set; } = null!;

        public Province Province { get; set; } = null!;
        

        
    }
}