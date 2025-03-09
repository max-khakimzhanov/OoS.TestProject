using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.Teacher.Update
{
    public class UpdateTeacherHandler : IRequestHandler<UpdateTeacherCommand, Result<TeacherDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateTeacherHandler> _logger;

        public UpdateTeacherHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            ILogger<UpdateTeacherHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TeacherDto>> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingTeacher = await _repositoryWrapper.TeacherRepository.GetFirstOrDefaultAsync(t => t.Id == request.Id);
                if (existingTeacher is null)
                {
                    string errorMsg = $"Teacher with ID {request.Id} not found.";
                    _logger.LogWarning(errorMsg);
                    return Result.Fail(errorMsg);
                }

                _mapper.Map(request.Teacher, existingTeacher);

                _repositoryWrapper.TeacherRepository.Update(existingTeacher);
                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                if (!resultIsSuccess)
                {
                    string errorMsg = "Failed to update teacher.";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                return Result.Ok(_mapper.Map<TeacherDto>(existingTeacher));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the teacher.");
                return Result.Fail("An unexpected error occurred.");
            }
        }
    }
}
