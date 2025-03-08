using AutoMapper;
using OoS.TestProject.BLL.Dto.Teacher;
using TeacherEntity = OoS.TestProject.DAL.Entities.Teacher;

namespace OoS.TestProject.BLL.Mapping.Teacher
{
    public class TeacherProfile : Profile
    {
        public TeacherProfile()
        {
            CreateMap<TeacherEntity, TeacherDto>();
            CreateMap<CreateTeacherDto, TeacherEntity>();
            CreateMap<UpdateTeacherDto, TeacherEntity>();
        }
    }
}
