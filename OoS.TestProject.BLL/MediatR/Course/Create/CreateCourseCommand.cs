using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Course;

namespace OoS.TestProject.BLL.MediatR.CourseEntity.Create
{
    public record CreateCourseCommand(CreateCourseDto Course) : IRequest<Result<CourseDto>>;
}
