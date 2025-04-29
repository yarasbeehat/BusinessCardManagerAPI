using System.Linq.Expressions;

namespace BusinessCardManager.Infrastructure.Interfaces
{
    public interface IBaseRepository<TEntity>
    {
        Task Add(TEntity entity);
        Task AddRange(IEnumerable<TEntity> entities);
        Task Update(TEntity entity);
        Task UpdateRange(IEnumerable<TEntity> entities);
        Task<TEntity> GetFirstOrDefaultWithInclude(
            Expression<Func<TEntity, bool>> predicate);
        Task<IQueryable<TEntity>> GetAllWithIncludes(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? page = null,
            int? pageSize = null);       
    }
}
