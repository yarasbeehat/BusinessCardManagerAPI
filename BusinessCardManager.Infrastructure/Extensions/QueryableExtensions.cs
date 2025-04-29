using System.Linq.Expressions;


namespace BusinessCardManager.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
        {
            if (page <= 0) page = 1;

            if (pageSize <= 0) pageSize = 10;

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }
    }
}
