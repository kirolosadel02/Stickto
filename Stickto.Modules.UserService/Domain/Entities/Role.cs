namespace Stickto.Modules.UserService.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }

        public string RoleName { get; set; } = null!;

        // Navigation property for all users in this role
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
