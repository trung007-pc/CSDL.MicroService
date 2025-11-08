namespace CSDL7.MasterService.Shared.Extensions;

public static class ObjectExtension
{
    
    /// <summary>
    /// Format date vn
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string FormatDateVN(this DateTime date)
    {
        return date.ToString("dd/MM/yyyy");
    }
}