using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OoS.TestProject.BLL.Services.Realizations;
using OoS.TestProject.BLL.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using OoS.TestProject.BLL.Services.Interfaces;

namespace OoS.TestProject.NUnitTest.ServiceTests
{
    [TestFixture]
    public class JwtServiceTests
    {
        private Mock<IOptions<JwtConfiguration>> _optionsMock;
        private Mock<ILogger<JwtService>> _loggerMock;
        private Mock<IClaimsService> _claimsServiceMock;
        private JwtService _jwtService;
        private JwtConfiguration _jwtConfiguration;

        [SetUp]
        public void Setup()
        {
            _jwtConfiguration = new JwtConfiguration
            {
                SecretKey = "VerySecretKey12345678901234567890",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenLifetime = 1
            };

            _optionsMock = new Mock<IOptions<JwtConfiguration>>();
            _optionsMock.Setup(o => o.Value).Returns(_jwtConfiguration);

            _loggerMock = new Mock<ILogger<JwtService>>();
            _claimsServiceMock = new Mock<IClaimsService>();

            _jwtService = new JwtService(_optionsMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GenerateTokenAsync_ValidClaims_ReturnsToken()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "User")
            };

            // Act
            var token = await _jwtService.GenerateTokenAsync(claims);

            // Assert
            Assert.NotNull(token);
            Assert.IsInstanceOf<string>(token);
            Assert.That(token, Does.StartWith("eyJ")); // Verify the token structure (JWT header)
        }

        [Test]
        public void GenerateTokenAsync_ExceptionThrown_LogsErrorAndRethrows()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "User")
            };

            var workingOptionsMock = new Mock<IOptions<JwtConfiguration>>();

            var nullKeyConfig = new JwtConfiguration
            {
                SecretKey = null,
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenLifetime = 1
            };

            workingOptionsMock.Setup(o => o.Value).Returns(nullKeyConfig);

            var jwtService = new JwtService(workingOptionsMock.Object, _loggerMock.Object);

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await jwtService.GenerateTokenAsync(claims));

            Assert.That(exception.ParamName, Is.EqualTo("s"));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GenerateTokenAsync_ValidToken_HasCorrectExpiration()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "User")
            };

            DateTime beforeTokenCreation = DateTime.UtcNow;

            // Act
            var token = await _jwtService.GenerateTokenAsync(claims);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.NotNull(jwtToken.ValidTo);

            DateTime expectedExpiration = beforeTokenCreation.AddHours(_jwtConfiguration.AccessTokenLifetime);
            TimeSpan difference = jwtToken.ValidTo - expectedExpiration;

            Assert.That(Math.Abs(difference.TotalSeconds), Is.LessThan(5));
        }

        [Test]
        public async Task GenerateTokenAsync_ValidToken_ContainsCorrectIssuerAndAudience()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "User")
            };

            // Act
            var token = await _jwtService.GenerateTokenAsync(claims);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.That(jwtToken.Issuer, Is.EqualTo(_jwtConfiguration.Issuer));
            Assert.That(jwtToken.Audiences.First(), Is.EqualTo(_jwtConfiguration.Audience));
        }
    }
}