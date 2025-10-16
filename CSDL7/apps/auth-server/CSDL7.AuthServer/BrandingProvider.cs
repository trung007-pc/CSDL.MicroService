using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace CSDL7.AuthServer;

[Dependency(ReplaceServices = true)]
public class BrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "CSDL7 Authentication Server";
}