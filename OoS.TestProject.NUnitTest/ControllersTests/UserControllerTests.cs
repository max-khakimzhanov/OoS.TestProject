using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using OoS.TestProject.BLL.Dto.User;
using OoS.TestProject.BLL.Services.Interfaces;
using OoS.TestProject.BLL.Services;
using OoS.TestProject.WebApi.Controllers;

namespace OoS.TestProject.NUnitTest.ControllersTests
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IRegisterService> _mockRegisterService;
        private Mock<ILoginService> _mockLoginService;
        private Mock<IOptions<JwtConfiguration>> _mockJwtConfig;
        private UserController _controller;
        private JwtConfiguration _jwtConfiguration;

        [SetUp]
        public void Setup()
        {
            _mockRegisterService = new Mock<IRegisterService>();
            _mockLoginService = new Mock<ILoginService>();
            _jwtConfiguration = new JwtConfiguration();
            _mockJwtConfig = new Mock<IOptions<JwtConfiguration>>();
            _mockJwtConfig.Setup(x => x.Value).Returns(_jwtConfiguration);

            _controller = new UserController(
                _mockRegisterService.Object,
                _mockLoginService.Object,
                _mockJwtConfig.Object);

            // Setup HttpContext for Login tests
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public async Task Register_WhenSuccessful_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "testuser",
                Password = "Password123!"
            };

            var successResult = Result.Ok("User registered successfully");
            _mockRegisterService.Setup(x => x.RegisterAsync(registerDto))
                .ReturnsAsync(successResult);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("User registered successfully", okResult.Value);
        }

        [Test]
        public async Task Register_WhenFailed_ReturnsBadRequestResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "testuser",
                Password = "Password123!"
            };

            var failedResult = Result.Fail("User with this UserName already exists.");
            _mockRegisterService.Setup(x => x.RegisterAsync(registerDto))
                .ReturnsAsync(failedResult);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult.Value);
        }

        [Test]
        public async Task Login_WhenFailed_ReturnsBadRequestResult()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "testuser",
                Password = "WrongPassword"
            };

            var failedResult = Result.Fail("Invalid login credentials.");
            _mockLoginService.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(failedResult);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult.Value);
        }
    }
}
