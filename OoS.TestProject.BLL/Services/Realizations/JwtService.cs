using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OoS.TestProject.BLL.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OoS.TestProject.BLL.Services.Realizations
{
    public class JwtService : IJwtService
    {
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IOptions<JwtConfiguration> options, ILogger<JwtService> logger)
        {
            _jwtConfiguration = options.Value;
            _logger = logger;
        }

        public async Task<string> GenerateTokenAsync(IEnumerable<Claim> claims)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var jwtToken = new JwtSecurityToken(
                    issuer: _jwtConfiguration.Issuer,
                    audience: _jwtConfiguration.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(_jwtConfiguration.AccessTokenLifetime),
                    signingCredentials: credentials
                );

                string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                _logger.LogInformation("Token generated successfully for issuer {Issuer}.", _jwtConfiguration.Issuer);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating token.");
                throw;
            }
        }
    }
}
