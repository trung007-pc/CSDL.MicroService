using CSDL7.MasterService.Entities.Districts;

using System;
using System.Collections.Generic;
using CSDL7.MasterService.Entities.Wards;

namespace CSDL7.MasterService.Entities.Wards
{
    public  class WardWithNavigationProperties
    {
        public Ward Ward { get; set; } = null!;

        public District District { get; set; } = null!;
        

        
    }
}