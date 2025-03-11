using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.User;
using OoS.TestProject.BLL.Services.Interfaces;
using UserEntity = OoS.TestProject.DAL.Entities.User;

namespace OoS.TestProject.BLL.Services.Realizations
{
    public class RegisterService : IRegisterService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger<RegisterService> _logger;
        private readonly IMapper _mapper;

        public RegisterService(
            UserManager<UserEntity> userManager,
            ILogger<RegisterService> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<string>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                if (registerDto == null)
                {
                    _logger.LogError("Register attempt with null RegisterDTO.");
                    return Result.Fail("Invalid register request.");
                }

                var existingUser = await _userManager.FindByNameAsync(registerDto.UserName);
                if (existingUser != null)
                {
                    return Result.Fail("User with this UserName already exists.");
                }

                var user = _mapper.Map<UserEntity>(registerDto);
                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Invalid register attempt for username: {UserName}", registerDto.UserName);
                    return Result.Fail("Invalid register credentials.");
                }
                return Result.Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during register.");
                return Result.Fail("An unexpected error occurred during register.");
            }
        }
    }
}
