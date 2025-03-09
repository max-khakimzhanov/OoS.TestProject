using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Teacher;

namespace OoS.TestProject.BLL.MediatR.Teacher.GetAll
{
    public record GetAllTeacherQuery : IRequest<Result<IEnumerable<TeacherDto>>>;
}
