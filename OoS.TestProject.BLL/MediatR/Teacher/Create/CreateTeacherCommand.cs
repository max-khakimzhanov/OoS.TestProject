using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Teacher;

namespace OoS.TestProject.BLL.MediatR.Teacher.Create
{
    public record CreateTeacherCommand(CreateTeacherDto Teacher) : IRequest<Result<TeacherDto>>;
}
