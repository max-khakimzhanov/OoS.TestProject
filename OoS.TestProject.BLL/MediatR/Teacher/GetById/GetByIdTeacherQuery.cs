using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Teacher;

namespace OoS.TestProject.BLL.MediatR.Teacher.GetById
{
    public record GetByIdTeacherQuery(int Id) : IRequest<Result<TeacherDto>>;
}
