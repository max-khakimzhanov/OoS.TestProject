using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.CourseEntity.GetAll
{
    public class GetAllCourseHandler : IRequestHandler<GetAllCourseQuery, Result<IEnumerable<CourseDto>>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCourseHandler> _logger;

        public GetAllCourseHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILogger<GetAllCourseHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<CourseDto>>> Handle(GetAllCourseQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var courses = await _repositoryWrapper.CourseRepository.GetAllAsync(
                    include: q => q.Include(c => c.Teacher).Include(c => c.Students));
                if (courses is null)
                {
                    string errorMsg = "No courses found.";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
                return Result.Ok(courseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses.");
                return Result.Fail("An error occurred while retrieving courses.");
            }
        }
    }
}
