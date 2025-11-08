using CSDL7.EmailService.Localization;
using Volo.Abp.Application.Services;

namespace CSDL7.EmailService;

public abstract class EmailAppService : ApplicationService
{
    protected EmailAppService()
    {
        LocalizationResource = typeof(EmailServiceResource);
        ObjectMapperContext = typeof(CSDL7EmailServiceModule);
    }
}