namespace CSDL7.MasterService.Shared.Extensions;

public static class RequestExtensions
{
    /// <summary>
    /// Read data from request body
    /// </summary>
    /// <param name="requestBody"></param>
    /// <param name="leaveOpen"></param>
    /// <returns></returns>
    public static async Task<string> ReadAsStringAsync(this Stream requestBody, bool leaveOpen = false)
    {
        using StreamReader reader = new(requestBody, leaveOpen: true);
        var bodyAsString = await reader.ReadToEndAsync();
        return bodyAsString;
    }
}