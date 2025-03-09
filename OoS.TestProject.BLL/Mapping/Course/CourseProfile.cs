using AutoMapper;
using OoS.TestProject.BLL.Dto.Course;
using CourseEntity = OoS.TestProject.DAL.Entities.Course;

namespace OoS.TestProject.BLL.Mapping.Course
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CourseEntity, CourseDto>().ReverseMap();
            CreateMap<CreateCourseDto, CourseEntity>();
            CreateMap<UpdateCourseDto, CourseEntity>();
        }
    }
}
