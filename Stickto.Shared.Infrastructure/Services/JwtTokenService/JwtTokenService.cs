using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stickto.Shared.Infrastructure.Options.Application;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Stickto.Shared.Infrastructure.Services.JwtTokenService
{ 
    public class JwtTokenService : IJwtTokenService
    {
        private readonly ApplicationOptions _applicationOptions;

        public JwtTokenService(IOptionsMonitor<ApplicationOptions> options)
        {
            _applicationOptions = options.CurrentValue;
        }

        public string GenerateToken(Guid userId, string email, string firstName, string lastName, string roleName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_applicationOptions.Token.Key);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.GivenName, firstName),
                new(ClaimTypes.Surname, lastName),
                new(ClaimTypes.Role, roleName),
                new("jti", Guid.NewGuid().ToString()),
                new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _applicationOptions.Token.Issuer,
                Audience = _applicationOptions.Origins.FirstOrDefault(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
