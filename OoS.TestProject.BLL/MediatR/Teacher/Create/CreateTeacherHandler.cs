using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.BLL.MediatR.Teacher.Create;
using OoS.TestProject.DAL.Repositories.Interfaces;
using TeacherModel = OoS.TestProject.DAL.Entities.Teacher;

namespace OoS.TestProject.BLL.MediatR.TeacherEntity.Create
{
    public class CreateTeacherHandler : IRequestHandler<CreateTeacherCommand, Result<TeacherDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateTeacherHandler> _logger;

        public CreateTeacherHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            ILogger<CreateTeacherHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TeacherDto>> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
        {
            if (request.Teacher is null)
            {
                string errorMsg = "Teacher data is null";
                _logger.LogError(errorMsg);
                return Result.Fail(errorMsg);
            }

            try
            {
                var newTeacher = _mapper.Map<TeacherModel>(request.Teacher);
                var entity = await _repositoryWrapper.TeacherRepository.CreateAsync(newTeacher);
                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                if (!resultIsSuccess)
                {
                    string errorMsg = "Failed to create a teacher";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                return Result.Ok(_mapper.Map<TeacherDto>(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a teacher.");
                return Result.Fail("An unexpected error occurred while creating the teacher.");
            }
        }
    }
}
