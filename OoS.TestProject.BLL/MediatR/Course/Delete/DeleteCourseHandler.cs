using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.CourseEntity.Delete
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILogger<DeleteCourseHandler> _logger;

        public DeleteCourseHandler(IRepositoryWrapper repositoryWrapper, ILogger<DeleteCourseHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var course = await _repositoryWrapper.CourseRepository.GetFirstOrDefaultAsync(c => c.Id == request.Id);
                if (course == null)
                {
                    string errorMsg = $"No course found by entered Id - {request.Id}";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                _repositoryWrapper.CourseRepository.Delete(course);
                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                if (resultIsSuccess)
                {
                    return Result.Ok(Unit.Value);
                }
                else
                {
                    string errorMsg = "Failed to delete course";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"An error occurred while deleting course with id {request.Id}";
                _logger.LogError(ex, errorMsg);
                return Result.Fail("An unexpected error occurred.");
            }
        }
    }
}
