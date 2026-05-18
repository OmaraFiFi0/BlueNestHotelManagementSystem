using BlueNest.Core.Contracts;
using BlueNest.Core.Entities;
using BlueNest.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Infrastructure.Repository
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly HotelDbContext _dbContext;

        public GenericRepository(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbContext.Set<TEntity>().ToListAsync();


        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? Filter = null,
            Expression<Func<TEntity, object>>? OrderByExp = null,
            Expression<Func<TEntity, object>>? OrderByDescExp = null,
            List<Expression<Func<TEntity, object>>>? InCludes = null)
        {
            var Query = _dbContext.Set<TEntity>().AsQueryable();

            if (Filter != null)
                Query = Query.Where(Filter);

            if (InCludes is not null)
            {
                foreach (var inclue in InCludes)
                    Query = Query.Include(inclue);
            }

            if (OrderByExp != null)
                Query = Query.OrderBy(OrderByExp);

            if (OrderByDescExp != null)
                Query = Query.OrderByDescending(OrderByDescExp);

            return await Query.ToListAsync();
        }


        public async Task<TEntity?> GetByIdAsync(TKey id) => await _dbContext.Set<TEntity>().FindAsync(id);


        public async Task AddAsync(TEntity entity) => await _dbContext.Set<TEntity>().AddAsync(entity);


        public void Delete(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);


        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);

        public async Task<TEntity?> GetByIdAsync(TKey id, Expression<Func<TEntity, bool>>? filter = null
            , List<Expression<Func<TEntity, object>>>? Includes = null)
        {
            var Query = _dbContext.Set<TEntity>().AsQueryable();
            if (filter is not null)
            {
                Query = Query.Where(filter);
            }

            if (Includes is not null)
            {
                foreach (var include in Includes)
                {
                    Query = Query.Include(include);
                }
            }

            return await Query.FirstOrDefaultAsync(E => E.Id!.Equals(id));
        }
    }
}
