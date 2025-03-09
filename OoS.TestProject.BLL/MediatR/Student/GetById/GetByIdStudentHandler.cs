using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OoS.TestProject.BLL.MediatR.Student.GetById
{
    public class GetByIdStudentHandler : IRequestHandler<GetByIdStudentQuery, Result<StudentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILogger<GetByIdStudentHandler> _logger;

        public GetByIdStudentHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILogger<GetByIdStudentHandler> logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<StudentDto>> Handle(GetByIdStudentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var student = await _repositoryWrapper.StudentRepository.GetFirstOrDefaultAsync(
                    predicate: s => s.Id == request.Id,
                    include: s => s.Include(s => s.Courses));

                if (student is null)
                {
                    string errorMsg = $"No student found by entered Id - {request.Id}";
                    _logger.LogError(errorMsg);
                    return Result.Fail(errorMsg);
                }

                return Result.Ok(_mapper.Map<StudentDto>(student));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student with ID {Id}", request.Id);
                return Result.Fail("An unexpected error occurred.");
            }
        }
    }
}
