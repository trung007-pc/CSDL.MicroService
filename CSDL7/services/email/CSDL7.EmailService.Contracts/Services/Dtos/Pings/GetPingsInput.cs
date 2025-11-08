using Volo.Abp.Application.Dtos;
using System;

namespace CSDL7.EmailService.Services.Dtos.Pings
{
    public class GetPingsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public int? ValueMin { get; set; }
        public int? ValueMax { get; set; }

        public GetPingsInput()
        {

        }
    }
}