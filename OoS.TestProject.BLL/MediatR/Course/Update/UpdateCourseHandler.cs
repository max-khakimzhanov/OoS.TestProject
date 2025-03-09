using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.CourseEntity.Update
{
    public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, Result<CourseDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCourseHandler> _logger;

        public UpdateCourseHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILogger<UpdateCourseHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CourseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingCourse = await _repositoryWrapper.CourseRepository.GetFirstOrDefaultAsync(c => c.Id == request.Id);
                if (existingCourse is null)
                {
                    string errorMsg = $"Course with ID {request.Id} not found.";
                    _logger.LogWarning(errorMsg);
                    return Result.Fail(errorMsg);
                }

                _mapper.Map(request.Course, existingCourse);

                _repositoryWrapper.CourseRepository.Update(existingCourse);
                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                if (!resultIsSuccess)
                {
                    string errorMsg = "Failed to update course.";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                return Result.Ok(_mapper.Map<CourseDto>(existingCourse));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the course.");
                return Result.Fail("An unexpected error occurred.");
            }
        }
    }
}
