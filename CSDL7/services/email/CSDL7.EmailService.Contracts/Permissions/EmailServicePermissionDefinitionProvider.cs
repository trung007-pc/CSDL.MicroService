using CSDL7.EmailService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace CSDL7.EmailService.Permissions;

public class EmailServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(EmailServicePermissions.GroupName);

        var pingPermission = myGroup.AddPermission(EmailServicePermissions.Pings.Default, L("Permission:Pings"));
        pingPermission.AddChild(EmailServicePermissions.Pings.Create, L("Permission:Create"));
        pingPermission.AddChild(EmailServicePermissions.Pings.Edit, L("Permission:Edit"));
        pingPermission.AddChild(EmailServicePermissions.Pings.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<EmailServiceResource>(name);
    }
}