namespace CSDL7.Shared.Extensions;

/// <summary>
/// Extension methods cho Collection
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Kiểm tra collection có null hoặc empty
    /// </summary>
    public static bool IsNullOrEmpty<T>(this ICollection<T>? source)
    {
        return source == null || source.Count == 0;
    }

    /// <summary>
    /// Thêm nhiều items vào collection
    /// </summary>
    public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            source.Add(item);
        }
    }

    /// <summary>
    /// Xóa các items thỏa điều kiện
    /// </summary>
    public static void RemoveWhere<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
        var itemsToRemove = source.Where(predicate).ToList();
        foreach (var item in itemsToRemove)
        {
            source.Remove(item);
        }
    }
}
