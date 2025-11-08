using CSDL7.MasterService.Permissions;
using CSDL7.MasterService.Localization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.Options;
using CSDL7.AdministrationService.Permissions;
using Volo.Abp.Account.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;
using Volo.Saas.Host.Blazor.Navigation;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using CSDL7.LanguageService.Localization;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;

namespace CSDL7.Blazor.Server.Navigation;

public class CSDL7MenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public CSDL7MenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<LanguageServiceResource>();

        //Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                CSDL7Menus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 0
            )
        );

        //HostDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                CSDL7Menus.HostDashboard,
                l["Menu:Dashboard"],
                "/HostDashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(AdministrationServicePermissions.Dashboard.Host)
        );

        //TenantDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                CSDL7Menus.TenantDashboard,
                l["Menu:Dashboard"],
                "/Dashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(AdministrationServicePermissions.Dashboard.Tenant)
        );

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 5;

        //Administration->Saas
        context.Menu.SetSubItemOrder(SaasHostMenus.GroupName, 3);

        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 2);

        //Administration->OpenIddict
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 3);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 4);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 5);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 6);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 7);

        // MasterService menu
        var masterServiceLocalizer = context.GetLocalizer<MasterServiceResource>();
        context.Menu.AddItem(
            new ApplicationMenuItem(
                name: CSDL7Menus.MasterService,
                displayName: masterServiceLocalizer["Menu:MasterService"],
                icon: "fa fa-database",
                order: 10
            ).AddItem(new ApplicationMenuItem(
                CSDL7Menus.Departments,
                context.GetLocalizer<MasterServiceResource>()["Menu:Departments"],
                url: "/departments",
                icon: "fa fa-file-alt",
                requiredPermissionName: MasterServicePermissions.Departments.Default)
            ).AddItem(
                new ApplicationMenuItem(
                    CSDL7Menus.Provinces,
                    context.GetLocalizer<MasterServiceResource>()["Menu:Provinces"],
                    url: "/provinces",
                    icon: "fa fa-file-alt",
                    requiredPermissionName: MasterServicePermissions.Provinces.Default)
            ).AddItem(
                new ApplicationMenuItem(
                    CSDL7Menus.Districts,
                    context.GetLocalizer<MasterServiceResource>()["Menu:Districts"],
                    url: "/districts",
                    icon: "fa fa-file-alt",
                    requiredPermissionName: MasterServicePermissions.Districts.Default)
            ).AddItem(
                new ApplicationMenuItem(
                    CSDL7Menus.Wards,
                    context.GetLocalizer<MasterServiceResource>()["Menu:Wards"],
                    url: "/wards",
                    icon: "fa fa-file-alt",
                    requiredPermissionName: MasterServicePermissions.Wards.Default)
            )
        );
    }

    private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "~";
        var uiResource = context.GetLocalizer<AbpUiResource>();
        var accountResource = context.GetLocalizer<AccountResource>();

        context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["MyAccount"], $"{authServerUrl.EnsureEndsWith('/')}Account/Manage", icon: "fa fa-cog", order: 1000, target: "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{authServerUrl.EnsureEndsWith('/')}Account/SecurityLogs", icon: "fa fa-user-shield", target: "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.Sessions", accountResource["Sessions"], url: $"{authServerUrl.EnsureEndsWith('/')}Account/Sessions", icon: "fa fa-clock", target: "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.Logout", uiResource["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue - 1000).RequireAuthenticated());

        return Task.CompletedTask;
    }
}