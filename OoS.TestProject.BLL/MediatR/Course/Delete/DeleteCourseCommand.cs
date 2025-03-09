using FluentResults;
using MediatR;

namespace OoS.TestProject.BLL.MediatR.CourseEntity.Delete
{
    public record DeleteCourseCommand(int Id) : IRequest<Result<Unit>>;
}
