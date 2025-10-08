using Stickto.Shared.Abstractions.Entities;

namespace Stickto.Modules.UserService.Domain.Entities
{
    public class User : AuditableEntity
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        // Foreign Key for Role (one-to-many: role -> users)
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        // Navigation Property for One-to-One Address
        public Address Address { get; set; } = null!;
    }
}
