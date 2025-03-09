using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Course;

namespace OoS.TestProject.BLL.MediatR.CourseEntity.Update
{
    public record UpdateCourseCommand(int Id, UpdateCourseDto Course) : IRequest<Result<CourseDto>>;
}
