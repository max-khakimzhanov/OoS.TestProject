using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Course;

namespace OoS.TestProject.BLL.MediatR.CourseEntity.GetAll
{
    public record GetAllCourseQuery : IRequest<Result<IEnumerable<CourseDto>>>;
}
