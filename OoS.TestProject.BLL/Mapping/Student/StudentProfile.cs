using AutoMapper;
using OoS.TestProject.BLL.Dto.Student;
using StudentEntity = OoS.TestProject.DAL.Entities.Student;

namespace OoS.TestProject.BLL.Mapping.Student
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<StudentEntity, StudentDto>();
            CreateMap<CreateStudentDto, StudentEntity>();
            CreateMap<UpdateStudentDto, StudentEntity>();
        }
    }
}
