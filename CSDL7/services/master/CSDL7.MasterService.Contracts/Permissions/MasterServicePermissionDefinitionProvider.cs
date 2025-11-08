using CSDL7.MasterService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace CSDL7.MasterService.Permissions;

public class MasterServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MasterServicePermissions.GroupName);

        var departmentPermission = myGroup.AddPermission(MasterServicePermissions.Departments.Default, L("Permission:Departments"));
        departmentPermission.AddChild(MasterServicePermissions.Departments.Create, L("Permission:Create"));
        departmentPermission.AddChild(MasterServicePermissions.Departments.Edit, L("Permission:Edit"));
        departmentPermission.AddChild(MasterServicePermissions.Departments.Delete, L("Permission:Delete"));

        var provincePermission = myGroup.AddPermission(MasterServicePermissions.Provinces.Default, L("Permission:Provinces"));
        provincePermission.AddChild(MasterServicePermissions.Provinces.Create, L("Permission:Create"));
        provincePermission.AddChild(MasterServicePermissions.Provinces.Edit, L("Permission:Edit"));
        provincePermission.AddChild(MasterServicePermissions.Provinces.Delete, L("Permission:Delete"));

        var districtPermission = myGroup.AddPermission(MasterServicePermissions.Districts.Default, L("Permission:Districts"));
        districtPermission.AddChild(MasterServicePermissions.Districts.Create, L("Permission:Create"));
        districtPermission.AddChild(MasterServicePermissions.Districts.Edit, L("Permission:Edit"));
        districtPermission.AddChild(MasterServicePermissions.Districts.Delete, L("Permission:Delete"));

        var wardPermission = myGroup.AddPermission(MasterServicePermissions.Wards.Default, L("Permission:Wards"));
        wardPermission.AddChild(MasterServicePermissions.Wards.Create, L("Permission:Create"));
        wardPermission.AddChild(MasterServicePermissions.Wards.Edit, L("Permission:Edit"));
        wardPermission.AddChild(MasterServicePermissions.Wards.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MasterServiceResource>(name);
    }
}