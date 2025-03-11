using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using OoS.TestProject.BLL.Services.Interfaces;
using OoS.TestProject.DAL.Entities;
using System.Security.Claims;

namespace OoS.TestProject.BLL.Services.Realizations
{
    public class ClaimsService : IClaimsService
    {
        private readonly ILogger<ClaimsService> _logger;

        public ClaimsService(ILogger<ClaimsService> logger)
        {
            _logger = logger;
        }

        public async Task<List<Claim>> CreateClaimsAsync(User user)
        {
            if (user == null)
            {
                _logger.LogError("User cannot be null.");
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                };

                _logger.LogInformation("Claims created successfully for user {UserName}.", user.UserName);
                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating claims for user {UserName}.", user.UserName);
                throw;
            }
        }
    }
}
