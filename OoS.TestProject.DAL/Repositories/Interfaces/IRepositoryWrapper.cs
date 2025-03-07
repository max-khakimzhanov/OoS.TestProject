using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OoS.TestProject.DAL.Repositories.Interfaces
{
    public interface IRepositoryWrapper
    {
        ICourseRepository CourseRepository { get; }
        IStudentRepository StudentRepository { get; }
        ITeacherRepository TeacherRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
