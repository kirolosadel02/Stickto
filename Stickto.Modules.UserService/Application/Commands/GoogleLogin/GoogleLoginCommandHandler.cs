using Google.Apis.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stickto.Modules.UserService.Domain.Entities;
using Stickto.Modules.UserService.Domain.Extensions;
using Stickto.Shared.Infrastructure;
using Stickto.Shared.Infrastructure.Services.JwtTokenService;

namespace Stickto.Modules.UserService.Application.Commands.GoogleLogin
{
    public class GoogleLoginCommandHandler(ApplicationDbContext context, IJwtTokenService jwtTokenService)
        : IRequestHandler<GoogleLoginCommand, GoogleLoginResponse>
    {
        public async Task<GoogleLoginResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify the Google ID token
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);

                if (payload == null)
                {
                    throw new UnauthorizedAccessException("Invalid Google token.");
                }

                // Check if user exists
                var user = await context.Set<User>()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == payload.Email, cancellationToken);

                if (user == null)
                {
                    // Create new user with Google authentication
                    var defaultRole = await context.Set<Role>()
                        .FirstOrDefaultAsync(r => r.RoleName == "User", cancellationToken);

                    if (defaultRole == null)
                    {
                        throw new InvalidOperationException("Default user role not found.");
                    }

                    var userId = Guid.NewGuid();

                    user = new User
                    {
                        Id = userId,
                        FirstName = payload.GivenName ?? "Unknown",
                        LastName = payload.FamilyName ?? "Unknown",
                        Email = payload.Email,
                        Password = null, // No password for Google auth
                        AuthProvider = "Google",
                        ExternalUserId = payload.Subject,
                        RoleId = defaultRole.Id,
                        Role = defaultRole
                    };

                    // Validate user state before saving
                    user.ValidateUserState();

                    // Create default address for OAuth users
                    var address = new Address
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Street = "Not Provided",
                        City = "Not Provided",
                        State = "Not Provided",
                        Country = "Not Provided",
                        ZipCode = "00000"
                    };

                    context.Set<User>().Add(user);
                    context.Set<Address>().Add(address);
                    await context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    // Update existing user with Google auth info if not already set
                    if (user.AuthProvider == null)
                    {
                        user.AuthProvider = "Google";
                        user.ExternalUserId = payload.Subject;
                        await context.SaveChangesAsync(cancellationToken);
                    }
                }

                // Generate JWT token
                var token = jwtTokenService.GenerateToken(
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role.RoleName
                );

                var userInfo = new GoogleUserInfo(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role.RoleName
                );

                return new GoogleLoginResponse(token, "Google login successful.", userInfo);
            }
            catch (InvalidJwtException)
            {
                throw new UnauthorizedAccessException("Invalid Google token.");
            }
        }
    }
}
