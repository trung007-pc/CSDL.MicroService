using Volo.Abp.Reflection;

namespace CSDL7.EmailService.Permissions;

public class EmailServicePermissions
{
    public const string GroupName = "EmailService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(EmailServicePermissions));
    }

    public static class Pings
    {
        public const string Default = GroupName + ".Pings";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}