using Stickto.Shared.Abstractions.Entities;

namespace Stickto.Modules.UserService.Domain.Entities
{
    public class Address : AuditableEntity
    {
        public Guid Id { get; set; }

        // Foreign Key for one-to-one
        public Guid UserId { get; set; }

        public string Street { get; set; } = null!;

        public string City { get; set; } = null!;

        public string State { get; set; } = null!;

        public string Country { get; set; } = null!;

        public string ZipCode { get; set; } = null!;

        // Navigation back to User (one-to-one)
        public User User { get; set; } = null!;
    }
}
