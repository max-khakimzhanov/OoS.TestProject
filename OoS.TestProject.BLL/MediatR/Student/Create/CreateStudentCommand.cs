using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Student;

namespace OoS.TestProject.BLL.MediatR.Student.Create
{
    public record CreateStudentCommand(CreateStudentDto Student) : IRequest<Result<StudentDto>>;
}
