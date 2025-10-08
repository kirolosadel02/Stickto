using Microsoft.EntityFrameworkCore;
using Stickto.Modules.UserService.Domain.Entities;
using Stickto.Shared.Infrastructure;

namespace Stickto.Modules.UserService.Infrastructure.Data
{
    public static class UserServiceSeeder
    {
        public static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync();
            
            // The roles are now seeded through the migration file
            // This method primarily ensures migrations are applied
            
            // Optionally verify that roles exist
            var rolesCount = await context.Set<Role>().CountAsync();
            if (rolesCount >= 2)
            {
                Console.WriteLine($"? Roles seeded successfully. Found {rolesCount} roles in database.");
            }
            else
            {
                Console.WriteLine("?? Warning: Expected roles were not found. Check database migrations.");
            }
        }
    }
}