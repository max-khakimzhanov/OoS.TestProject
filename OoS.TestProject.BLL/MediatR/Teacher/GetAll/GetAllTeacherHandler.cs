using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.DAL.Repositories.Interfaces;

namespace OoS.TestProject.BLL.MediatR.Teacher.GetAll
{
    public class GetAllTeacherHandler : IRequestHandler<GetAllTeacherQuery, Result<IEnumerable<TeacherDto>>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllTeacherHandler> _logger;

        public GetAllTeacherHandler(
            IRepositoryWrapper repositoryWrapper, 
            IMapper mapper, 
            ILogger<GetAllTeacherHandler> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<TeacherDto>>> Handle(GetAllTeacherQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var teachers = await _repositoryWrapper.TeacherRepository.GetAllAsync(
                    include: q => q.Include(t => t.Courses));
                if (teachers is null)
                {
                    string errorMsg = "No teachers found.";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                var teacherDtos = _mapper.Map<IEnumerable<TeacherDto>>(teachers);
                return Result.Ok(teacherDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teachers.");
                return Result.Fail("An error occurred while retrieving teachers.");
            }
        }
    }
}
