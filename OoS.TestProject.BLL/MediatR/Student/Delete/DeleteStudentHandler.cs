using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.Student.Delete
{
    public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILogger<DeleteStudentHandler> _logger;

        public DeleteStudentHandler(IRepositoryWrapper repositoryWrapper, ILogger<DeleteStudentHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var student = await _repositoryWrapper.StudentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Id);
                if (student == null)
                {
                    string errorMsg = $"No student found by entered Id - {request.Id}";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                _repositoryWrapper.StudentRepository.Delete(student);
                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                return resultIsSuccess
                    ? Result.Ok(Unit.Value)
                    : Result.Fail("Failed to delete student");
            }
            catch (Exception ex)
            {
                string errorMsg = $"An error occurred while deleting student with id {request.Id}";
                _logger.LogError(ex, errorMsg);
                return Result.Fail("An unexpected error occurred.");
            }
        }
    }
}
