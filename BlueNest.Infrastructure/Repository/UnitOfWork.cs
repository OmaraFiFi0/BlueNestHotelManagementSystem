using BlueNest.Core.Contracts;
using BlueNest.Core.Entities;
using BlueNest.Infrastructure.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Infrastructure.Repository
{
    
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelDbContext _dbContext;

        public UnitOfWork(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private Dictionary<Type, object> _repositories = [];
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var type = typeof(TEntity);

            if (_repositories.TryGetValue(type, out var repository))
            {
                return (IGenericRepository<TEntity, TKey>)repository;
            }

            var newRepository = new GenericRepository<TEntity, TKey>(_dbContext);

            _repositories[type]=newRepository;

            return newRepository;

        }

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
        
    }
}
