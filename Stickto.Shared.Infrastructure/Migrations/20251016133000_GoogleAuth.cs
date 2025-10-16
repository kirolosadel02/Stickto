using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stickto.Shared.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GoogleAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthProvider",
                schema: "user_service",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalUserId",
                schema: "user_service",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthProvider",
                schema: "user_service",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExternalUserId",
                schema: "user_service",
                table: "Users");
        }
    }
}
