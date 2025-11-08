using Volo.Saas.Tenant;
using Volo.Saas.Host;
using Localization.Resources.AbpUi;
using CSDL7.SaasService.Localization;
using Volo.Abp.Application;
using Volo.Abp.Commercial.SuiteTemplates;
using Volo.Abp.Authorization;
using Volo.Abp.Domain;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.UI;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Localization.Resources.AbpLocalization;

namespace CSDL7.SaasService;

[DependsOn(
    typeof(SaasTenantApplicationContractsModule),
    typeof(SaasHostApplicationContractsModule),
    typeof(AbpValidationModule),
    typeof(VoloAbpCommercialSuiteTemplatesModule),
    typeof(AbpUiModule),
    typeof(AbpAuthorizationAbstractionsModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpDddDomainSharedModule)
    )]
public class CSDL7SaasServiceContractsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CSDL7SaasServiceContractsModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<SaasServiceResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource), typeof(AbpUiResource))
                .AddVirtualJson("/Localization/SaasService");
            
            options.Languages.Add(new LanguageInfo("vi", "vi", "Vietnamese"));
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("SaasService", typeof(SaasServiceResource));
        });
    }
}
