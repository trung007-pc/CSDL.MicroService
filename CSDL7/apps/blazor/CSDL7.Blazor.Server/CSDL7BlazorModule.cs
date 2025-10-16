using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Extensions.Options;
using CSDL7.Blazor.Server.Components;
using CSDL7.Blazor.Server.Navigation;
using Polly;
using Prometheus;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.LinkUsers;
using Volo.Abp.Account.Public.Web.Impersonation;
using Volo.Abp.Account.Pro.Admin.Blazor.Server;
using Volo.Abp.Account.Pro.Public.Blazor.Server;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Server;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AutoMapper;
using Volo.Abp.AspNetCore.Components.Server.LeptonXTheme;
using Volo.Abp.AspNetCore.Components.Server.LeptonXTheme.Bundling;
using Volo.Abp.AspNetCore.Components.Web.LeptonXTheme;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonX;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonX.Bundling;
using Volo.Abp.AspNetCore.Components.Web.LeptonXTheme;
using Volo.Abp.LeptonX.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Http.Client.Web;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Pro.Blazor.Server;
using Volo.Abp.Identity.Pro.Blazor;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.Localization;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Pro.Blazor.Server;
using Volo.Abp.TextTemplateManagement;
using Volo.Abp.TextTemplateManagement.Blazor.Server;
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.Blazor.Server;
using CSDL7.AuditLoggingService;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LanguageManagement.Blazor.Server;
using CSDL7.LanguageService;
using CSDL7.LanguageService.Localization;
using Volo.Abp.Gdpr;
using Volo.Abp.Gdpr.Blazor.Server;
using CSDL7.GdprService;
using Volo.Saas.Host;
using Volo.Saas.Host.Blazor;
using Volo.Saas.Host.Blazor.Server;
using Volo.Saas.Tenant;
using Volo.Saas.Tenant.Blazor.Server;
using CSDL7.SaasService;
using CSDL7.IdentityService;
using CSDL7.AdministrationService;
using CSDL7.Blazor.Server.HealthChecks;

namespace CSDL7.Blazor.Server;

[DependsOn(
    typeof(AbpAccountAdminHttpApiClientModule),
    typeof(AbpAccountAdminBlazorServerModule),
    typeof(SaasHostHttpApiClientModule),
    typeof(SaasHostBlazorServerModule),
    typeof(SaasTenantHttpApiClientModule),
    typeof(SaasTenantBlazorServerModule),
    typeof(CSDL7SaasServiceContractsModule),
    typeof(CSDL7IdentityServiceContractsModule),
    typeof(CSDL7AdministrationServiceContractsModule),
    typeof(AbpOpenIddictProBlazorServerModule),
    typeof(AbpOpenIddictProHttpApiClientModule),
    typeof(CSDL7AuditLoggingServiceContractsModule),
    typeof(AbpAuditLoggingHttpApiClientModule),
    typeof(AbpAuditLoggingBlazorServerModule),
    typeof(TextTemplateManagementBlazorServerModule),
    typeof(TextTemplateManagementHttpApiClientModule),
    typeof(LanguageManagementBlazorServerModule),
    typeof(LanguageManagementHttpApiClientModule),
    typeof(CSDL7LanguageServiceContractsModule),
    typeof(AbpGdprBlazorServerModule),
    typeof(AbpGdprHttpApiClientModule),
    typeof(CSDL7GdprServiceContractsModule),
    typeof(AbpAccountPublicWebImpersonationModule),
    typeof(AbpAccountPublicBlazorServerModule),
    typeof(AbpAccountPublicHttpApiClientModule),
    typeof(AbpIdentityHttpApiClientModule),
    typeof(AbpIdentityProBlazorServerModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpEventBusRabbitMqModule),
    typeof(AbpAspNetCoreMvcClientModule),
    typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule),
    typeof(AbpHttpClientWebModule),
    typeof(AbpHttpClientIdentityModelWebModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAspNetCoreComponentsServerLeptonXThemeModule),
    typeof(AbpAspNetCoreMvcUiLeptonXThemeModule),
    typeof(AbpSettingManagementHttpApiClientModule),
    typeof(AbpPermissionManagementHttpApiClientModule),
    typeof(AbpFeatureManagementHttpApiClientModule),
    typeof(AbpStudioClientAspNetCoreModule)
)]
public class CSDL7BlazorModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigureDataAnnotations(context);
        PreConfigureHttpClient();

        PreConfigure<AbpAspNetCoreComponentsWebOptions>(options =>
        {
            options.IsBlazorWebApp = true;
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        var redis = CreateRedisConnection(configuration);

        // Add services to the container.
        context.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        ConfigureAutoMapper();
        ConfigurePII(configuration);
        ConfigureLocalization(hostingEnvironment);
        ConfigureBundling();
        ConfigureMultiTenancy();
        ConfigureDistributedCache(configuration);
        ConfigureUrls(configuration);
        ConfigureAuthentication(context, configuration);
        ConfigureDataProtection(context, configuration, redis);
        ConfigureNavigation(configuration);
        ConfigureToolbar();
        ConfigureLeptonXTheme();
        ConfigureImpersonation();
        ConfigureBlazorise(context);
        ConfigureAccountLinkUser(configuration);
        ConfigureRouter(context); 
        ConfigureHealthChecks(context);
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (!env.IsDevelopment())
        {
            app.Use((ctx, next) =>
            {
                /* This application should act like it is always called as HTTPS.
                 * Because it will work in a HTTPS url in production,
                 * but the HTTPS is stripped out in Ingress controller.
                 */
                ctx.Request.Scheme = "https";
                return next();
            });
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        
        if (env.IsDevelopment())
        {
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None,
                Secure = CookieSecurePolicy.Always
            });
        }
        
        app.UseCorrelationId();
        app.UseRouting();
        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();
        app.UseAbpSecurityHeaders();
        app.UseHttpMetrics();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseMultiTenancy();
        app.UseAbpSerilogEnrichers();
        app.UseAuthorization();
        app.UseConfiguredEndpoints(builder =>
        {
            builder.MapMetrics();
            builder.MapRazorComponents<Components.App>()
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(builder.ServiceProvider.GetRequiredService<IOptions<AbpRouterOptions>>().Value.AdditionalAssemblies.ToArray());
        });
    }
    
    private static void PreConfigureDataAnnotations(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(LanguageServiceResource),
                typeof(CSDL7BlazorModule).Assembly
            );
        });
    }
    
    private void PreConfigureHttpClient()
    {
        PreConfigure<AbpHttpClientBuilderOptions>(options =>
        {
            options.ProxyClientBuildActions.Add((remoteServiceName, clientBuilder) =>
            {
                clientBuilder.AddTransientHttpErrorPolicy(policyBuilder =>
                    policyBuilder.WaitAndRetryAsync(
                        4,
                        i => TimeSpan.FromSeconds(Math.Pow(2, i))
                    )
                );
            });
        });
    }
    
    private ConnectionMultiplexer CreateRedisConnection(IConfiguration configuration)
    {
        return ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<CSDL7BlazorAutoMapperProfile>(validate: true);
        });
    }

    private void ConfigureHealthChecks(ServiceConfigurationContext context)
    {
        context.Services.AddWebHealthChecks();
    }
    
    private void ConfigurePII(IConfiguration configuration)
    {
        if (configuration.GetValue<bool>(configuration["App:EnablePII"]))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        }
    }
    
    private void ConfigureLocalization(IWebHostEnvironment hostingEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CSDL7BlazorModule>();
        });
        
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<CSDL7BlazorModule>(hostingEnvironment.ContentRootPath);
            });
        }

    }
    
    private void ConfigureBundling()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            // MVC UI
            options.StyleBundles.Configure(
                LeptonXThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );

            options.ScriptBundles.Configure(
                LeptonXThemeBundles.Scripts.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-scripts.js");
                }
            );

            // Blazor UI
            options.StyleBundles.Configure(
                BlazorLeptonXThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }
    
    private void ConfigureMultiTenancy()
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = true;
        });
    }
    
    private void ConfigureDistributedCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = configuration["AbpDistributedCache:KeyPrefix"]!;
        });
    }
    
    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        }); 

        Configure<AbpAccountLinkUserOptions>(options =>
        {
            options.LoginUrl = configuration["AuthServer:Authority"];
        });
    }
    
    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
            })
            .AddAbpOpenIdConnect("oidc", options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>(configuration["AuthServer:RequireHttpsMetadata"]);
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                
                options.ClientId = configuration["AuthServer:ClientId"];
                options.ClientSecret = configuration["AuthServer:ClientSecret"];

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                
                options.TokenValidationParameters.ValidIssuers = new[]
                {
                    configuration["AuthServer:Authority"].EnsureEndsWith('/'),
                    configuration["AuthServer:MetaAddress"].EnsureEndsWith('/')
                }; 

                options.Scope.Add("roles");
                options.Scope.Add("email");
                options.Scope.Add("phone");
                options.Scope.Add("AuthServer");
                options.Scope.Add("IdentityService");
                options.Scope.Add("AdministrationService");
                options.Scope.Add("SaasService");
                options.Scope.Add("AuditLoggingService");
                options.Scope.Add("GdprService");
                options.Scope.Add("LanguageService");
            });
        
        if (Convert.ToBoolean(configuration["AuthServer:IsOnK8s"]))
        {
            context.Services.Configure<OpenIdConnectOptions>("oidc", options =>
            {
                options.MetadataAddress = configuration["AuthServer:MetaAddress"].EnsureEndsWith('/') + ".well-known/openid-configuration";

                var previousOnRedirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
                options.Events.OnRedirectToIdentityProvider = async ctx =>
                {
                    // Intercept the redirection so the browser navigates to the right URL in your host
                    ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"].EnsureEndsWith('/') + "connect/authorize";
                    if (previousOnRedirectToIdentityProvider != null)
                    {
                        await previousOnRedirectToIdentityProvider(ctx);
                    }
                };
                var previousOnRedirectToIdentityProviderForSignOut = options.Events.OnRedirectToIdentityProviderForSignOut;
                options.Events.OnRedirectToIdentityProviderForSignOut = async ctx =>
                {
                    // Intercept the redirection for signout so the browser navigates to the right URL in your host
                    ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"].EnsureEndsWith('/') + "connect/endsession";
                    if (previousOnRedirectToIdentityProviderForSignOut != null)
                    {
                        await previousOnRedirectToIdentityProviderForSignOut(ctx);
                    }
                };
            });
        }
    }
    
    private static void ConfigureDataProtection(ServiceConfigurationContext context, IConfiguration configuration, ConnectionMultiplexer redis)
    {
        context.Services
            .AddDataProtection()
            .SetApplicationName(configuration["DataProtection:ApplicationName"]!)
            .PersistKeysToStackExchangeRedis(redis, configuration["DataProtection:Keys"]);
    }
    
    private void ConfigureNavigation(IConfiguration configuration)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new CSDL7MenuContributor(configuration));
        });
    }
    
    private void ConfigureToolbar()
    {
        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new CSDL7ToolbarContributor());
        });
    }
    
    private void ConfigureLeptonXTheme()
    {
         Configure<LeptonXThemeOptions>(options =>
         {
             options.DefaultStyle = LeptonXStyleNames.System;
        });
         
         Configure<LeptonXThemeMvcOptions>(options =>
         {
             options.ApplicationLayout = LeptonXMvcLayouts.SideMenu;
         });

         Configure<LeptonXThemeBlazorOptions>(options =>
         {
             options.Layout = LeptonXBlazorLayouts.SideMenu;
         });
    }
    
    private void ConfigureImpersonation()
    {
        Configure<SaasHostBlazorOptions>(options =>
        {
            options.EnableTenantImpersonation = true;
        });
        Configure<AbpIdentityProBlazorOptions>(options =>
        {
            options.EnableUserImpersonation = true;
        });
    }
    
    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        context.Services
            .AddBlazorise(options =>
            {
                // TODO (IMPORTANT): To use Blazorise, you need a license key. Get your license key directly from Blazorise, follow  the instructions at https://abp.io/faq#how-to-get-blazorise-license-key
                //options.ProductToken = "Your Product Token";
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
    }
    
    private void ConfigureAccountLinkUser(IConfiguration configuration)
    {
        Configure<AbpAccountLinkUserOptions>(options => { options.LoginUrl = configuration["AuthServer:Authority"]; });
    }
    
    private void ConfigureRouter(ServiceConfigurationContext context)
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(CSDL7BlazorModule).Assembly;
        });
    }
}
