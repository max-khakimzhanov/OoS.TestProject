using AutoMapper;
using FluentResults;
using MediatR;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace OoS.TestProject.BLL.MediatR.Student.Update
{
    public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, Result<StudentDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateStudentHandler> _logger;

        public UpdateStudentHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            ILogger<UpdateStudentHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<StudentDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingStudent = await _repositoryWrapper.StudentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Id);
                if (existingStudent is null)
                {
                    string errorMsg = $"Student with ID {request.Id} not found.";
                    _logger.LogWarning(errorMsg);
                    return Result.Fail(errorMsg);
                }

                _mapper.Map(request.Student, existingStudent);

                _repositoryWrapper.StudentRepository.Update(existingStudent);
                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                if (!resultIsSuccess)
                {
                    string errorMsg = "Failed to update student.";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                return Result.Ok(_mapper.Map<StudentDto>(existingStudent));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the student.");
                return Result.Fail("An unexpected error occurred.");
            }
        }
    }
}
