using System.Linq.Expressions;
using TN3.Tenant.Admin.Domain.Commons.Interfaces.Enties;

namespace CSDL7.MasterService.Shared.Extensions;

public static class LinqExtension
{
    /// <summary>
    ///  Order By If
    /// </summary>
    /// <param name="y"></param>
    /// <param name="condition"></param>
    /// <param name="keySelector"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static IQueryable<TSource> OrderByIf<TSource, TKey>(this  IQueryable<TSource> y, 
        bool condition, Expression<Func<TSource, TKey>> keySelector)
    {
        return condition ? y.OrderBy(keySelector) : y;
    }
    
    /// <summary>
    /// Order By Desc If
    /// </summary>
    /// <param name="y"></param>
    /// <param name="condition"></param>
    /// <param name="keySelector"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static IQueryable<TSource> OrderByDescIf<TSource, TKey>(this  IQueryable<TSource> y, 
        bool condition, Expression<Func<TSource, TKey>> keySelector)
    {
        return condition ? y.OrderByDescending(keySelector) : y;
    }
    
    /// <summary>
    /// If Exist
    /// </summary>
    /// <param name="query"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IQueryable<TSource> IfExist<TSource>(this  IQueryable<TSource> query) where TSource : IShortDeletableEntity
    {
        return query.Where(x => !x.IsDeleted);
    }
}

