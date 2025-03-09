using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.Student.GetAll
{
    public class GetAllStudentHandler : IRequestHandler<GetAllStudentQuery, Result<IEnumerable<StudentDto>>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllStudentHandler> _logger;

        public GetAllStudentHandler(
            IRepositoryWrapper repositoryWrapper, 
            IMapper mapper, 
            ILogger<GetAllStudentHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<StudentDto>>> Handle(GetAllStudentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var students = await _repositoryWrapper.StudentRepository.GetAllAsync(
                    include: q => q.Include(s => s.Courses));
                if (students is null)
                {
                    string errorMsg = "No students found.";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(students);
                return Result.Ok(studentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving students.");
                return Result.Fail("An error occurred while retrieving students.");
            }
        }
    }
}
