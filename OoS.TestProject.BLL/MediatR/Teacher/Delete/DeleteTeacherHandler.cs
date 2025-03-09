using FluentResults;
using MediatR;
using OoS.TestProject.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace OoS.TestProject.BLL.MediatR.Teacher.Delete
{
    public class DeleteTeacherHandler : IRequestHandler<DeleteTeacherCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILogger<DeleteTeacherHandler> _logger;

        public DeleteTeacherHandler(IRepositoryWrapper repositoryWrapper, ILogger<DeleteTeacherHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var teacher = await _repositoryWrapper.TeacherRepository.GetFirstOrDefaultAsync(t => t.Id == request.Id);
                if (teacher == null)
                {
                    string errorMsg = $"No teacher found by entered Id - {request.Id}";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                _repositoryWrapper.TeacherRepository.Delete(teacher);
                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                return resultIsSuccess
                    ? Result.Ok(Unit.Value)
                    : Result.Fail("Failed to delete teacher");
            }
            catch (Exception ex)
            {
                string errorMsg = $"An error occurred while deleting teacher with id {request.Id}";
                _logger.LogError(ex, errorMsg);
                return Result.Fail("An unexpected error occurred.");
            }
        }
    }
}
