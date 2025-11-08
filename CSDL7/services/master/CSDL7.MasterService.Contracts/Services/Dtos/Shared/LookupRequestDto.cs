using Volo.Abp.Application.Dtos;

namespace CSDL7.MasterService.Services.Dtos.Shared
{
    public class LookupRequestDto : PagedResultRequestDto
    {
        public string? Filter { get; set; }

        public LookupRequestDto()
        {
            MaxResultCount = MaxMaxResultCount;
        }
    }
}