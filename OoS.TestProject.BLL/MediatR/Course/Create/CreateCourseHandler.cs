using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.DAL.Entities;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.CourseEntity.Create
{
    public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Result<CourseDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCourseHandler> _logger;

        public CreateCourseHandler(
            IRepositoryWrapper repositoryWrapper, 
            IMapper mapper, 
            ILogger<CreateCourseHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CourseDto>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            if (request.Course is null)
            {
                string errorMsg = "Course data is null";
                _logger.LogError(errorMsg);
                return Result.Fail(errorMsg);
            }

            try
            {
                var newCourse = _mapper.Map<Course>(request.Course);
                var entity = await _repositoryWrapper.CourseRepository.CreateAsync(newCourse);
                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                if (!resultIsSuccess)
                {
                    string errorMsg = "Failed to create a course";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                return Result.Ok(_mapper.Map<CourseDto>(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a course.");
                return Result.Fail("An unexpected error occurred while creating the course.");
            }
        }
    }
}
