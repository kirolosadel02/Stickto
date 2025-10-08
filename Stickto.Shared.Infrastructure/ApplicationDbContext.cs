using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Stickto.Shared.Infrastructure
{
    /// <summary>
    /// The ApplicationDbContext class is the main class that coordinates Entity Framework functionality for a given data model.
    /// This class is created by deriving from the Microsoft.EntityFrameworkCore.DbContext class.
    /// </summary>
    public sealed class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// Constructor for ApplicationDbContext that takes DbContextOptions.
        /// </summary>
        /// <param name="options">The options to be used by a DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but before the model has been locked down and used to initialize the context.
        /// The default implementation of this method does nothing, but it can be overridden in a derived class such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically define extension methods on this object that allow you to configure aspects of the model that are specific to a given database.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configurations from current assembly
            _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            // Apply configurations from loaded assemblies that contain "Stickto.Modules" in their name
            var moduleAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName != null && a.FullName.Contains("Stickto.Modules"))
                .ToArray();

            foreach (var assembly in moduleAssemblies)
            {
                _ = modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            }
        }
    }
}
