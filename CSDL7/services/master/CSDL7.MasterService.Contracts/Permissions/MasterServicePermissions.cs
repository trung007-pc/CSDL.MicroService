using Volo.Abp.Reflection;

namespace CSDL7.MasterService.Permissions;

public class MasterServicePermissions
{
    public const string GroupName = "MasterService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(MasterServicePermissions));
    }

    public static class Departments
    {
        public const string Default = GroupName + ".Departments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Provinces
    {
        public const string Default = GroupName + ".Provinces";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Districts
    {
        public const string Default = GroupName + ".Districts";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Wards
    {
        public const string Default = GroupName + ".Wards";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}