using Volo.Abp.Application.Dtos;
using System;

namespace CSDL7.MasterService.Services.Dtos.Districts
{
    public class GetDistrictsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public string? Code { get; set; }
        public Guid? ProvinceId { get; set; }

        public GetDistrictsInput()
        {

        }
    }
}