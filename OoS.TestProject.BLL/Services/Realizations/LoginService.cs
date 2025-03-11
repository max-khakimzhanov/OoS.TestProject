using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OoS.TestProject.BLL.Dto.User;
using OoS.TestProject.BLL.Services.Interfaces;
using UserEntity = OoS.TestProject.DAL.Entities.User;

namespace OoS.TestProject.BLL.Services.Realizations
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger<LoginService> _logger;
        private readonly IClaimsService _claimsService;
        private readonly IJwtService _jwtService;

        public LoginService(
            UserManager<UserEntity> userManager,
            ILogger<LoginService> logger,
            IClaimsService claimsService,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _logger = logger;
            _claimsService = claimsService;
            _jwtService = jwtService;
        }
        public async Task<Result<string>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                {
                    _logger.LogError("Login attempt with null LoginDTO.");
                    return Result.Fail("Invalid login request.");
                }

                var user = await _userManager.FindByNameAsync(loginDto.UserName);

                if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    _logger.LogWarning("Invalid login attempt for UserName: {UserName}", loginDto.UserName);
                    return Result.Fail("Invalid login credentials.");
                }

                var claims = await _claimsService.CreateClaimsAsync(user);
                var token = await _jwtService.GenerateTokenAsync(claims);
                return Result.Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return Result.Fail("An unexpected error occurred during login.");
            }
        }
    }
}
