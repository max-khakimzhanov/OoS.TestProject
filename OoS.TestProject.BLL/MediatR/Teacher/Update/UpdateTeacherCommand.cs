using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Teacher;

namespace OoS.TestProject.BLL.MediatR.Teacher.Update
{
    public record UpdateTeacherCommand(int Id, UpdateTeacherDto Teacher) : IRequest<Result<TeacherDto>>;
}
