using Volo.Abp;

namespace CSDL7.Shared.Exceptions;

/// <summary>
/// Base exception cho business logic
/// </summary>
public class BusinessException : UserFriendlyException
{
    public BusinessException(
        string message,
        string? code = null,
        string? details = null,
        Exception? innerException = null)
        : base(message, code, details, innerException)
    {
    }
}
