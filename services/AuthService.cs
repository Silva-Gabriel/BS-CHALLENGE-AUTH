using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using domain.enums;
using domain.models;
using Microsoft.IdentityModel.Tokens;

namespace services
{
    public class AuthService : IAuthService
    {
        public string GenerateToken(Authentication authRequest, int role, string secretKey, DateTime expiration)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenDescriptor = GetTokenDescriptor(GetClaimsIdentity(authRequest, role), secretKey, expiration);
            var token = handler.CreateToken(tokenDescriptor);
            var tokenString = handler.WriteToken(token);

            return tokenString;
        }
        private static ClaimsIdentity GetClaimsIdentity(Authentication authRequest, int role)
        {
            var CI = new ClaimsIdentity();
            CI.AddClaim(new Claim(ClaimTypes.Name, authRequest.User));
            CI.AddClaim(new Claim(ClaimTypes.Role, Enum.GetName(typeof(Role), role) ?? Role.GUEST.ToString()));

            return CI;
        }

        private static SecurityTokenDescriptor GetTokenDescriptor(ClaimsIdentity claimsIdentity, string secretKey, DateTime expiration)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = expiration,
                SigningCredentials = credentials
            };

            return tokenDescriptor;
        }
    }
}