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

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T?>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
