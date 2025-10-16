using Volo.Abp.Reflection;

namespace CSDL7.AuditLoggingService.Permissions;

public class AuditLoggingServicePermissions
{
    public const string GroupName = "AuditLoggingService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AuditLoggingServicePermissions));
    }
}