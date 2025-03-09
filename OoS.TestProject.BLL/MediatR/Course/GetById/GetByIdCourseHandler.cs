using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.CourseEntity.GetById
{
    public class GetByIdCourseHandler : IRequestHandler<GetByIdCourseQuery, Result<CourseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILogger<GetByIdCourseHandler> _logger;

        public GetByIdCourseHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILogger<GetByIdCourseHandler> logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<CourseDto>> Handle(GetByIdCourseQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var course = await _repositoryWrapper.CourseRepository.GetFirstOrDefaultAsync(
                    predicate: c => c.Id == request.Id,
                    include: c => c.Include(c => c.Teacher).Include(c => c.Students));

                if (course is null)
                {
                    string errorMsg = $"No course found by entered Id - {request.Id}";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                return Result.Ok(_mapper.Map<CourseDto>(course));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course with ID {Id}", request.Id);
                return Result.Fail("An unexpected error occurred.");
            }
        }
    }
}
