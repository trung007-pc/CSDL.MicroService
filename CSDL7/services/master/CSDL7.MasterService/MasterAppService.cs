using CSDL7.MasterService.Localization;
using Volo.Abp.Application.Services;

namespace CSDL7.MasterService;

public abstract class MasterAppService : ApplicationService
{
    protected MasterAppService()
    {
        LocalizationResource = typeof(MasterServiceResource);
        ObjectMapperContext = typeof(CSDL7MasterServiceModule);
    }
}