using Volo.Abp.AspNetCore.Components;
using CSDL7.LanguageService.Localization;

namespace CSDL7.Blazor.Server;

public abstract class CSDL7ComponentBase : AbpComponentBase
{
    protected CSDL7ComponentBase()
    {
        LocalizationResource = typeof(LanguageServiceResource);
    }
}
