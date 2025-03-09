using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OoS.TestProject.DAL.Persistence;

namespace OoS.TestProject.DAL.Repositories.Realizations
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly OoSTestProjectDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public RepositoryBase(OoSTestProjectDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }
        public async Task<T> CreateAsync(T entity)
        {
            var createdEntity = (await _dbSet.AddAsync(entity)).Entity;
            return createdEntity;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? predicate = default,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
        {
            return await GetQueryable(predicate, include).FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = default,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
        {
            return await GetQueryable(predicate, include).ToListAsync();
        }

        private IQueryable<T> GetQueryable(
            Expression<Func<T, bool>>? predicate = default,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (include is not null)
            {
                query = include(query);
            }

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            return query;
        }
    }
}
