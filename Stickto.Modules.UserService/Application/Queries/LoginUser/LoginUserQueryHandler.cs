using MediatR;
using Microsoft.EntityFrameworkCore;
using Stickto.Modules.UserService.Domain.Entities;
using Stickto.Shared.Infrastructure;
using Stickto.Shared.Infrastructure.Services.JwtTokenService;
using System.Security.Cryptography;
using System.Text;

namespace Stickto.Modules.UserService.Application.Queries.LoginUser
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, LoginUserResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginUserQueryHandler(ApplicationDbContext context, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginUserResponse> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            // Hash the provided password
            var hashedPassword = HashPassword(request.Password);

            // Find user with role
            var user = await _context.Set<User>()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == hashedPassword, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Role.RoleName
            );

            var userInfo = new UserInfo(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role.RoleName
            );

            return new LoginUserResponse(token, "Login successful.", userInfo);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}