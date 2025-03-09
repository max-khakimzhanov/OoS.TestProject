using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Student;

namespace OoS.TestProject.BLL.MediatR.Student.GetById
{
    public record GetByIdStudentQuery(int Id) : IRequest<Result<StudentDto>>;
}
