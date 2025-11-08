using Volo.Abp.Application.Dtos;

namespace CSDL7.EmailService.Services.Dtos.Shared
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