using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace CSDL7.Blazor.Server;

[Dependency(ReplaceServices = true)]
public class CSDL7BrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "CSDL7";
}