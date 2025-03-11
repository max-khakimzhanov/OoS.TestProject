using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OoS.TestProject.BLL.Dto.User;
using OoS.TestProject.BLL.Services;
using OoS.TestProject.BLL.Services.Interfaces;
using OoS.TestProject.WebApi.Extensions;

namespace OoS.TestProject.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        private readonly ILoginService _loginService;
        private readonly IOptions<JwtConfiguration> _jwtConfig;

        public UserController(
            IRegisterService registerService,
            ILoginService loginService,
            IOptions<JwtConfiguration> jwtConfig)
        {
            _registerService = registerService;
            _loginService = loginService;
            _jwtConfig = jwtConfig;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _registerService.RegisterAsync(registerDto);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _loginService.LoginAsync(loginDto);

            if (result.IsFailed)
                return BadRequest(result.Errors);

            var jwtToken = result.Value;
            HttpContext.AppendTokenToCookie(jwtToken, _jwtConfig);

            return Ok(new { token = jwtToken });
        }
    }
}
