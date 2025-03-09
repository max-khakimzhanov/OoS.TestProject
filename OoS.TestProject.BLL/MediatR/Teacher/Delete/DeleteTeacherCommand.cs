using FluentResults;
using MediatR;

namespace OoS.TestProject.BLL.MediatR.Teacher.Delete
{
    public record DeleteTeacherCommand(int Id) : IRequest<Result<Unit>>;
}
