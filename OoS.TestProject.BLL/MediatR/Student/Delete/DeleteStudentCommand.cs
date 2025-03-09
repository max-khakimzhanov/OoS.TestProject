using FluentResults;
using MediatR;

namespace OoS.TestProject.BLL.MediatR.Student.Delete
{
    public record DeleteStudentCommand(int Id) : IRequest<Result<Unit>>;
}
