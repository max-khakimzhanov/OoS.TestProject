using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.BLL.MediatR.Student.Create;
using StudentModel = OoS.TestProject.DAL.Entities.Student;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.StudentEntity.Create
{
    public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Result<StudentDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateStudentHandler> _logger;

        public CreateStudentHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            ILogger<CreateStudentHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<StudentDto>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            if (request.Student is null)
            {
                string errorMsg = "Student data is null";
                _logger.LogError(errorMsg);
                return Result.Fail(errorMsg);
            }

            try
            {
                var newStudent = _mapper.Map<StudentModel>(request.Student);
                var entity = await _repositoryWrapper.StudentRepository.CreateAsync(newStudent);
                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                if (!resultIsSuccess)
                {
                    string errorMsg = "Failed to create a student";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                return Result.Ok(_mapper.Map<StudentDto>(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a student.");
                return Result.Fail("An unexpected error occurred while creating the student.");
            }
        }
    }
}