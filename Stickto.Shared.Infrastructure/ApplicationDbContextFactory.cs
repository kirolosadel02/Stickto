using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Stickto.Shared.Infrastructure
{
    /// <summary>
    /// Factory class to create <see cref="ApplicationDbContext"/> for design-time services like EF Core migrations.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ApplicationDbContext"/> with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments to use for creating the context.</param>
        /// <returns>A new instance of <see cref="ApplicationDbContext"/>.</returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Get the current directory of the Infrastructure project
            var currentDirectory = Directory.GetCurrentDirectory();

            // Check environment variable to determine which appsettings file to load
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Load the configuration from the appsettings file based on the environment
            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory) // Set the base path to the WebApi directory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Default settings
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true) // Environment-specific settings
                .Build();

            // Retrieve the connection string from ApplicationSettings
            var connectionString = configuration.GetSection("ApplicationSettings")["ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in the configuration file.");
            }

            // Create a DbContextOptionsBuilder and configure it with the connection string
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });

            // Return a new ApplicationDbContext instance
            return new ApplicationDbContext(builder.Options);
        }
    }

}
