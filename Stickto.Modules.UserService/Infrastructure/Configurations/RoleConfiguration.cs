using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stickto.Modules.UserService.Domain.Entities;
using Stickto.Modules.UserService.Domain.Enums;

namespace Stickto.Modules.UserService.Infrastructure.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", "user_service");

            builder.HasKey(r => r.Id);

            // Configure identity to start from 1 and increment by 1
            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);

            builder.Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data with explicit IDs matching the enum values
            builder.HasData(
                new Role { Id = (int)UserRole.User, RoleName = "User" },
                new Role { Id = (int)UserRole.Admin, RoleName = "Admin" }
            );
        }
    }
}