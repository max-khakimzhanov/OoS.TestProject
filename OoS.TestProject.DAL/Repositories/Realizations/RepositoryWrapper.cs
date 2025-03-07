using Microsoft.Extensions.DependencyInjection;
using OoS.TestProject.DAL.Persistence;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace OoS.TestProject.DAL.Repositories.Realizations
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly OoSTestProjectDbContext _dbContext;

        public RepositoryWrapper(IServiceProvider serviceProvider, OoSTestProjectDbContext dbContext)
        {
            _serviceProvider = serviceProvider;
            _dbContext = dbContext;
        }

        public ICourseRepository CourseRepository => GetRepository<ICourseRepository>();
        public IStudentRepository StudentRepository => GetRepository<IStudentRepository>();
        public ITeacherRepository TeacherRepository => GetRepository<ITeacherRepository>();

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        private T GetRepository<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
