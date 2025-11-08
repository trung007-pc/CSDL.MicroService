namespace CSDL7.Shared.Constants;

/// <summary>
/// Các hằng số chung cho toàn bộ hệ thống
/// </summary>
public static class SharedConstants
{
    /// <summary>
    /// Độ dài tối đa cho các trường
    /// </summary>
    public static class MaxLength
    {
        public const int Name = 256;
        public const int Code = 128;
        public const int Description = 512;
        public const int Url = 2048;
        public const int PhoneNumber = 20;
        public const int Email = 256;
    }

    /// <summary>
    /// Các pattern validation
    /// </summary>
    public static class ValidationPatterns
    {
        public const string Email = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public const string PhoneNumber = @"^[0-9+()-\s]+$";
        public const string Code = @"^[A-Z0-9_-]+$";
    }

    /// <summary>
    /// Các giá trị mặc định
    /// </summary>
    public static class DefaultValues
    {
        public const int PageSize = 10;
        public const int MaxPageSize = 100;
    }
}
