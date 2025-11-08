using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CSDL7.MasterService.Shared.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Remove space of text
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string TrimSpace(this string text)
    {
        if (text.IsNullOrEmpty())
        {
            return text;
        }

        return text.Trim();
    }


    /// <summary>
    /// Remove Diacritics
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveDiacritics(this string text)
    {
        return string.Concat(
            text.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
        ).Normalize(NormalizationForm.FormC);
    }
    
    /// <summary>
    /// Convert Guid to short guid
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static string ToShortString(this Guid guid)
    {
        var base64Guid = Convert.ToBase64String(guid.ToByteArray());

        // Replace URL unfriendly characters
        base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

        // Remove the trailing ==
        return base64Guid.Substring(0, base64Guid.Length - 2);
    }

    /// <summary>
    /// Convert short guid to guid
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Guid FromShortString(this string str)
    {
        str = str.Replace('_', '/').Replace('-', '+');
        var byteArray = Convert.FromBase64String(str + "==");
        return new Guid(byteArray);
    }
    
    /// <summary>
    /// Try parse decimal
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public static decimal TryParseDecimal(this string input)
    {
        if (input.IsNullOrEmpty()) new InvalidCastException();
        
        decimal result;
        if (decimal.TryParse(input, out result))
        {
            return result;
        }
        
        throw new InvalidCastException();
    }

    /// <summary>
    /// Convert string to object
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static dynamic ToJson(this string input)
    {
        string formattedJson = input; // Giữ nguyên nếu không format được
        try
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            // Kiểm tra xem chuỗi có vẻ là JSON không
            input = input.Trim();
            if ((input.StartsWith("{") && input.EndsWith("}")) || 
                (input.StartsWith("[") && input.EndsWith("]")))
            {
                object jsonObject = JsonConvert.DeserializeObject<object>(input);
                formattedJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            }
            else if (input.StartsWith("\"") && input.EndsWith("\""))
            {
                // Trường hợp chuỗi bị escape
                string unescapedString = JsonConvert.DeserializeObject<string>(input);
                object jsonObject = JsonConvert.DeserializeObject<object>(unescapedString);
                formattedJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            }
        }
        catch (JsonException ex)
        {
            formattedJson = $"Error formatting JSON: {ex.Message} | Raw input: {input}";
        }
    
        return formattedJson;
    }

    #region Make Beautifull for url
    /// <summary>
    /// Make beautifull for url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string ToFriendlyUrl(this string url)
    {
        string str = RemoveAccent(url).ToLower();

        str = str.Trim();
        // Xóa ký tự không hợp lệ
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        // Chuyển nhiều khoảng trắng thành một dấu gạch ngang
        str = Regex.Replace(str, @"\s+", "-").Trim();
        // Cắt xén chuỗi dài
        str = CutString(str, 65);
        // Loại bỏ dấu gạch ngang cuối cùng
        str = Regex.Replace(str, @"^-", "");
        return str;
    }

    private static string CutString(string str, int maxLength)
    {
        if (str.Length <= maxLength)
            return str;

        var words = str.Split('-');
        var result = new StringBuilder();
        var length = 0;

        foreach (var word in words)
        {
            if (length + word.Length > maxLength)
                break;

            if (result.Length > 0)
                result.Append("-");

            result.Append(word);
            length += word.Length;
        }

        return result.ToString();
    }
    private static string RemoveAccent(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                if (c == 'đ') stringBuilder.Append('d');
                else if (c == 'Đ') stringBuilder.Append('D');
                else stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
    #endregion
}