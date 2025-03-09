using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Student;

namespace OoS.TestProject.BLL.MediatR.Student.GetAll
{
    public record GetAllStudentQuery : IRequest<Result<IEnumerable<StudentDto>>>;
}
