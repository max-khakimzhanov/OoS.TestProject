using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.Teacher.GetById
{
    public class GetByIdTeacherHandler : IRequestHandler<GetByIdTeacherQuery, Result<TeacherDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILogger<GetByIdTeacherHandler> _logger;

        public GetByIdTeacherHandler(
            IMapper mapper, 
            IRepositoryWrapper repositoryWrapper, 
            ILogger<GetByIdTeacherHandler> logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<TeacherDto>> Handle(GetByIdTeacherQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var teacher = await _repositoryWrapper.TeacherRepository.GetFirstOrDefaultAsync(
                    predicate: t => t.Id == request.Id,
                    include: t => t.Include(t => t.Courses));

                if (teacher is null)
                {
                    string errorMsg = $"No teacher found by entered Id - {request.Id}";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                return Result.Ok(_mapper.Map<TeacherDto>(teacher));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher with ID {Id}", request.Id);
                return Result.Fail("An unexpected error occurred.");
            }
        }
    }
}
