using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Course;

namespace OoS.TestProject.BLL.MediatR.CourseEntity.GetById
{
    public record GetByIdCourseQuery(int Id) : IRequest<Result<CourseDto>>;
}
