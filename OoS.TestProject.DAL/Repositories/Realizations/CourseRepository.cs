using OoS.TestProject.DAL.Entities;
using OoS.TestProject.DAL.Persistence;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OoS.TestProject.DAL.Repositories.Realizations
{
    public class CourseRepository : RepositoryBase<Course>, ICourseRepository
    {
        public CourseRepository(OoSTestProjectDbContext dbContext) : base(dbContext) { }
    }
}
