namespace CSDL7.Shared.Extensions;

/// <summary>
/// Extension methods cho String
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Kiểm tra chuỗi có null hoặc rỗng
    /// </summary>
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Kiểm tra chuỗi có null hoặc chỉ chứa khoảng trắng
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Cắt chuỗi nếu vượt quá độ dài tối đa
    /// </summary>
    public static string? Truncate(this string? value, int maxLength)
    {
        if (value.IsNullOrEmpty() || value!.Length <= maxLength)
        {
            return value;
        }

        return value.Substring(0, maxLength);
    }

    /// <summary>
    /// Chuyển chuỗi thành dạng Title Case
    /// </summary>
    public static string? ToTitleCase(this string? value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return value;
        }

        var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(value!.ToLower());
    }

    /// <summary>
    /// Loại bỏ dấu tiếng Việt
    /// </summary>
    public static string? RemoveVietnameseTones(this string? value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return value;
        }

        var vietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        for (int i = 1; i < vietnameseSigns.Length; i++)
        {
            for (int j = 0; j < vietnameseSigns[i].Length; j++)
            {
                value = value!.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
            }
        }

        return value;
    }
}
