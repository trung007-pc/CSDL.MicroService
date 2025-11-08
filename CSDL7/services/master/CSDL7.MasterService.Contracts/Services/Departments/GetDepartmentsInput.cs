using Volo.Abp.Application.Dtos;
using System;

namespace CSDL7.MasterService.Services.Dtos.Departments
{
    public class GetDepartmentsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? Name { get; set; }

        public GetDepartmentsInput()
        {

        }
    }
}