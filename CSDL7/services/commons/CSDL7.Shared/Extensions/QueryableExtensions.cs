using System.Linq.Expressions;

namespace CSDL7.Shared.Extensions;

/// <summary>
/// Extension methods cho IQueryable
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Phân trang
    /// </summary>
    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int skipCount, int maxResultCount)
    {
        return query.Skip(skipCount).Take(maxResultCount);
    }

    /// <summary>
    /// Điều kiện có thể null
    /// </summary>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }
}
