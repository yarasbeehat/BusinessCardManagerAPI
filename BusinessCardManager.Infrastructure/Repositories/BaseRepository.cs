using System.Linq.Expressions;
using BusinessCardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using BusinessCardManager.Infrastructure.Context;
using BusinessCardManager.Infrastructure.Interfaces;

namespace BusinessCardManager.Infrastructure.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly BusinessCardManagerContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(BusinessCardManagerContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task Add(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRange(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }
        public async Task<IQueryable<TEntity>> GetAllWithIncludes(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? page = null, int? pageSize = null)
        {
            IQueryable<TEntity> results = _dbSet;

            if (predicate != null)
                results = results.Where(predicate);

            if (page.HasValue && pageSize.HasValue && page.Value > 0 && pageSize.Value > 0)
                results = results.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            if (orderBy != null)
                results = orderBy(results);

            return results;
        }

        public async Task<TEntity> GetFirstOrDefaultWithInclude(
            Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> results = _dbSet;    
            return await results.FirstOrDefaultAsync(predicate);
        }

        public async Task Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        
    }
}
