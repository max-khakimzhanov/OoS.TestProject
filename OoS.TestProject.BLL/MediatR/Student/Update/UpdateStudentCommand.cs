using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Student;

namespace OoS.TestProject.BLL.MediatR.Student.Update
{
    public record UpdateStudentCommand(int Id, UpdateStudentDto Student) : IRequest<Result<StudentDto>>;
}
