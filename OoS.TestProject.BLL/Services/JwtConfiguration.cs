namespace OoS.TestProject.BLL.Services
{
    public class JwtConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int AccessTokenLifetime { get; set; }
        public int RefreshTokenLifetime { get; set; }
    }
}
