using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.User;
using OoS.TestProject.BLL.Services.Interfaces;
using OoS.TestProject.BLL.Services.Realizations;
using OoS.TestProject.DAL.Entities;
using System.Security.Claims;

namespace OoS.TestProject.NUnitTest.ServiceTests
{
    [TestFixture]
    public class LoginServiceTests
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<ILogger<LoginService>> _loggerMock;
        private Mock<IClaimsService> _claimsServiceMock;
        private Mock<IJwtService> _jwtServiceMock;
        private LoginService _loginService;

        [SetUp]
        public void SetUp()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _loggerMock = new Mock<ILogger<LoginService>>();
            _claimsServiceMock = new Mock<IClaimsService>();
            _jwtServiceMock = new Mock<IJwtService>();

            _loginService = new LoginService(
                _userManagerMock.Object,
                _loggerMock.Object,
                _claimsServiceMock.Object,
                _jwtServiceMock.Object);
        }

        [Test]
        public async Task LoginAsync_WithNullDto_ShouldReturnFail()
        {
            // Act
            var result = await _loginService.LoginAsync(null);

            // Assert
            Assert.That(result.IsFailed, Is.True);
            Assert.That(result.Errors[0].Message, Is.EqualTo("Invalid login request."));
        }

        [Test]
        public async Task LoginAsync_WithInvalidUser_ShouldReturnFail()
        {
            // Arrange
            var loginDto = new LoginDto { UserName = "InvalidUser", Password = "password123" };
            _userManagerMock.Setup(um => um.FindByNameAsync(loginDto.UserName)).ReturnsAsync((User)null);

            // Act
            var result = await _loginService.LoginAsync(loginDto);

            // Assert
            Assert.That(result.IsFailed, Is.True);
            Assert.That(result.Errors[0].Message, Is.EqualTo("Invalid login credentials."));
        }

        [Test]
        public async Task LoginAsync_WithIncorrectPassword_ShouldReturnFail()
        {
            // Arrange
            var user = new User { UserName = "ValidUser" };
            var loginDto = new LoginDto { UserName = "ValidUser", Password = "wrongpassword" };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginDto.UserName)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(false);

            // Act
            var result = await _loginService.LoginAsync(loginDto);

            // Assert
            Assert.That(result.IsFailed, Is.True);
            Assert.That(result.Errors[0].Message, Is.EqualTo("Invalid login credentials."));
        }

        [Test]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var user = new User { UserName = "ValidUser" };
            var loginDto = new LoginDto { UserName = "ValidUser", Password = "correctpassword" };
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) };
            var token = "mockJwtToken";

            _userManagerMock.Setup(um => um.FindByNameAsync(loginDto.UserName)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);
            _claimsServiceMock.Setup(cs => cs.CreateClaimsAsync(user)).ReturnsAsync(claims);
            _jwtServiceMock.Setup(js => js.GenerateTokenAsync(claims)).ReturnsAsync(token);

            // Act
            var result = await _loginService.LoginAsync(loginDto);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(token));
        }

        [Test]
        public async Task LoginAsync_WhenExceptionOccurs_ShouldReturnFail()
        {
            // Arrange
            var loginDto = new LoginDto { UserName = "ValidUser", Password = "correctpassword" };
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _loginService.LoginAsync(loginDto);

            // Assert
            Assert.That(result.IsFailed, Is.True);
            Assert.That(result.Errors[0].Message, Is.EqualTo("An unexpected error occurred during login."));
        }
    }
}
