namespace CSDL7.Shared.Extensions;

/// <summary>
/// Extension methods cho DateTime
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Lấy ngày đầu tiên của tháng
    /// </summary>
    public static DateTime GetFirstDayOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// Lấy ngày cuối cùng của tháng
    /// </summary>
    public static DateTime GetLastDayOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
    }

    /// <summary>
    /// Kiểm tra có phải cuối tuần không
    /// </summary>
    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    /// Lấy ngày đầu tuần (Thứ 2)
    /// </summary>
    public static DateTime GetStartOfWeek(this DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Lấy ngày cuối tuần (Chủ nhật)
    /// </summary>
    public static DateTime GetEndOfWeek(this DateTime date)
    {
        return date.GetStartOfWeek().AddDays(6);
    }

    /// <summary>
    /// Chuyển về đầu ngày (00:00:00)
    /// </summary>
    public static DateTime StartOfDay(this DateTime date)
    {
        return date.Date;
    }

    /// <summary>
    /// Chuyển về cuối ngày (23:59:59.999)
    /// </summary>
    public static DateTime EndOfDay(this DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }
}
