using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.User;
using OoS.TestProject.BLL.Services.Realizations;
using OoS.TestProject.DAL.Entities;

namespace OoS.TestProject.NUnitTest.ServiceTests
{
    [TestFixture]
    public class RegisterServiceTests
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<ILogger<RegisterService>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private RegisterService _registerService;

        [SetUp]
        public void SetUp()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _loggerMock = new Mock<ILogger<RegisterService>>();
            _mapperMock = new Mock<IMapper>();
            _registerService = new RegisterService(_userManagerMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task RegisterAsync_WithNullDto_ShouldReturnFail()
        {
            // Arrange

            // Act
            var result = await _registerService.RegisterAsync(null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid register request.", result.Errors[0].Message);
        }

        [Test]
        public async Task RegisterAsync_WithExistingUser_ShouldReturnFail()
        {
            // Arrange
            var registerDto = new RegisterDto { UserName = "existingUser", Password = "Password123" };
            _userManagerMock.Setup(um => um.FindByNameAsync(registerDto.UserName))
                .ReturnsAsync(new User());

            // Act
            var result = await _registerService.RegisterAsync(registerDto);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("User with this UserName already exists.", result.Errors[0].Message);
        }

        [Test]
        public async Task RegisterAsync_WithInvalidUserCreation_ShouldReturnFail()
        {
            // Arrange
            var registerDto = new RegisterDto { UserName = "newUser", Password = "Password123" };
            _userManagerMock.Setup(um => um.FindByNameAsync(registerDto.UserName))
                .ReturnsAsync((User)null);
            _mapperMock.Setup(m => m.Map<User>(registerDto)).Returns(new User { UserName = registerDto.UserName });
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error creating user" }));

            // Act
            var result = await _registerService.RegisterAsync(registerDto);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid register credentials.", result.Errors[0].Message);
        }

        [Test]
        public async Task RegisterAsync_WithValidUser_ShouldReturnSuccess()
        {
            // Arrange
            var registerDto = new RegisterDto { UserName = "newUser", Password = "Password123" };
            _userManagerMock.Setup(um => um.FindByNameAsync(registerDto.UserName))
                .ReturnsAsync((User)null);
            _mapperMock.Setup(m => m.Map<User>(registerDto)).Returns(new User { UserName = registerDto.UserName });
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _registerService.RegisterAsync(registerDto);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("User registered successfully", result.Value);
        }

        [Test]
        public async Task RegisterAsync_WhenExceptionOccurs_ShouldReturnFail()
        {
            // Arrange
            var registerDto = new RegisterDto { UserName = "exceptionUser", Password = "Password123" };
            _userManagerMock.Setup(um => um.FindByNameAsync(registerDto.UserName))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _registerService.RegisterAsync(registerDto);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An unexpected error occurred during register.", result.Errors[0].Message);
        }
    }
}
