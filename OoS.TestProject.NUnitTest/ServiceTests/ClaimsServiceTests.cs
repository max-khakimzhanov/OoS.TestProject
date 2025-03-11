using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Services.Realizations;
using OoS.TestProject.DAL.Entities;
using System.Security.Claims;

namespace OoS.TestProject.NUnitTest.ServiceTests
{
    [TestFixture]
    public class ClaimsServiceTests
    {
        private ClaimsService _claimsService;
        private Mock<ILogger<ClaimsService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<ClaimsService>>();
            _claimsService = new ClaimsService(_loggerMock.Object);
        }

        [Test]
        public void CreateClaimsAsync_WithNullUser_ShouldThrowArgumentNullException()
        {
            // Arrange
            User user = null;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _claimsService.CreateClaimsAsync(user));
            Assert.That(ex.ParamName, Is.EqualTo("user"));
        }

        [Test]
        public async Task CreateClaimsAsync_WithValidUser_ShouldReturnClaimsList()
        {
            // Arrange
            var user = new User { Id = "123", UserName = "testUser" };

            // Act
            var claims = await _claimsService.CreateClaimsAsync(user);

            // Assert
            Assert.That(claims, Is.Not.Null);
            Assert.That(claims, Has.Count.EqualTo(3));
            Assert.That(claims, Has.Exactly(1).Matches<Claim>(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id));
            Assert.That(claims, Has.Exactly(1).Matches<Claim>(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub && c.Value == user.UserName));
        }

        [Test]
        public void CreateClaimsAsync_WhenExceptionOccurs_ShouldThrowException()
        {
            // Arrange
            var user = new User { Id = "123", UserName = null };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _claimsService.CreateClaimsAsync(user));
        }
    }
}
