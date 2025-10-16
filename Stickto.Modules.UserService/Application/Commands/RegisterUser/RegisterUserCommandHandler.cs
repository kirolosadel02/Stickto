using MediatR;
using Microsoft.EntityFrameworkCore;
using Stickto.Modules.UserService.Domain.Entities;
using Stickto.Modules.UserService.Domain.Extensions;
using Stickto.Shared.Infrastructure;
using System.Security.Cryptography;
using System.Text;

namespace Stickto.Modules.UserService.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly ApplicationDbContext _context;

        public RegisterUserCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check if user already exists
            var existingUser = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            // Validate role exists
            var roleExists = await _context.Set<Role>()
                .AnyAsync(r => r.Id == request.RoleId, cancellationToken);

            if (!roleExists)
            {
                throw new InvalidOperationException("Invalid role specified.");
            }

            // Hash password (required for local auth)
            var hashedPassword = HashPassword(request.Password);

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = hashedPassword,
                AuthProvider = null, // Local authentication
                ExternalUserId = null,
                RoleId = request.RoleId,
                CreatedOn = DateTime.UtcNow
            };

            // Validate user state before saving
            user.ValidateUserState();

            // Create address
            var address = new Address
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Street = request.Street,
                City = request.City,
                State = request.State,
                Country = request.Country,
                ZipCode = request.ZipCode,
                CreatedOn = DateTime.UtcNow
            };

            _context.Set<User>().Add(user);
            _context.Set<Address>().Add(address);

            await _context.SaveChangesAsync(cancellationToken);

            return new RegisterUserResponse(user.Id, "User registered successfully.");
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}