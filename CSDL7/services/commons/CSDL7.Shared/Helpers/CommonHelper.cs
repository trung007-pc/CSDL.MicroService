namespace CSDL7.Shared.Helpers;

/// <summary>
/// Helper cho xử lý các thao tác chung
/// </summary>
public static class CommonHelper
{
    /// <summary>
    /// Tạo mã code ngẫu nhiên
    /// </summary>
    public static string GenerateRandomCode(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Tạo slug từ chuỗi
    /// </summary>
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        // Chuyển về chữ thường
        text = text.ToLower();

        // Loại bỏ dấu tiếng Việt
        text = RemoveVietnameseTones(text);

        // Thay thế khoảng trắng và ký tự đặc biệt bằng dấu gạch ngang
        text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "-").Trim('-');
        text = System.Text.RegularExpressions.Regex.Replace(text, @"-+", "-");

        return text;
    }

    /// <summary>
    /// Loại bỏ dấu tiếng Việt
    /// </summary>
    private static string RemoveVietnameseTones(string text)
    {
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
                text = text.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
            }
        }

        return text;
    }

    /// <summary>
    /// Định dạng số điện thoại
    /// </summary>
    public static string? FormatPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return phoneNumber;
        }

        // Loại bỏ ký tự không phải số
        var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

        // Format: 0xxx xxx xxx
        if (digitsOnly.Length == 10 && digitsOnly.StartsWith("0"))
        {
            return $"{digitsOnly.Substring(0, 4)} {digitsOnly.Substring(4, 3)} {digitsOnly.Substring(7)}";
        }

        return phoneNumber;
    }
}
