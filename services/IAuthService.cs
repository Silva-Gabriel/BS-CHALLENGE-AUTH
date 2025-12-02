using domain.models;

namespace services
{
    public interface IAuthService
    {
        public string GenerateToken(Authentication authRequest, int role, string secretKey, DateTime expiration);
    }
}