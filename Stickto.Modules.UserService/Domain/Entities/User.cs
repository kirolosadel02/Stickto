using Stickto.Shared.Abstractions.Entities;

namespace Stickto.Modules.UserService.Domain.Entities
{
    public class User : AuditableEntity
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Password { get; set; }

        // External authentication provider (e.g., "Google", null for local auth)
        public string? AuthProvider { get; set; }

        // External provider user ID
        public string? ExternalUserId { get; set; }

        // Foreign Key for Role (one-to-many: role -> users)
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        // Navigation Property for One-to-One Address
        public Address Address { get; set; } = null!;
    }
}
