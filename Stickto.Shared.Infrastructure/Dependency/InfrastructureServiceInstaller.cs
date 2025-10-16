using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Scrutor;
using Stickto.Shared.Infrastructure.Configurations.Authentication;
using Stickto.Shared.Infrastructure.Handlers;
using Stickto.Shared.Infrastructure.Interceptors;
using Stickto.Shared.Infrastructure.Options.Application;
using Stickto.Shared.Infrastructure.Services.JwtTokenService;
using System.Diagnostics;
using System.Reflection;
using MediatR;

namespace Stickto.Shared.Infrastructure.Dependency
{
    /// <summary>
    /// Represents a service installer for infrastructure services.
    /// This class is responsible for registering infrastructure-related
    /// services into the DI container.
    /// </summary>
#pragma warning disable CA1506 // Class Coupling
    public class InfrastructureServiceInstaller : IServiceInstaller
    {
        /// <summary>
        /// Installs services into the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="applicationOptions">The application options setup.</param>
        public void Install(IServiceCollection services, ApplicationOptions applicationOptions)
        {
            // Exception Handler Register
            _ = services.AddExceptionHandler<GlobalExceptionHandler>();
            _ = services.AddProblemDetails();
            _ = services.AddHttpContextAccessor();

            // Get module assemblies dynamically
            var moduleAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName != null && a.FullName.Contains("Stickto.Modules"))
                .ToArray();

            // Add MediatR
            _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(moduleAssemblies));

            // Authentication and Authorization
            _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer()
                .AddGoogle();
            
            _ = services.ConfigureOptions<ConfigureJwtToken>();
            _ = services.ConfigureOptions<ConfigureGoogleAuth>();
            _ = services.AddScoped<IJwtTokenService, JwtTokenService>();

            _ = services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
            });

            // Scrutor auto register services and repositories (excluding MediatR types and records)
            _ = services.Scan(selector => selector
            .FromAssemblies(moduleAssemblies)
            .AddClasses(classes => classes.Where(type => 
                type.Namespace != null
                && !type.Namespace.Contains("Specifications", StringComparison.InvariantCultureIgnoreCase)
                && !type.Namespace.Contains("ModelDtos", StringComparison.InvariantCultureIgnoreCase)
                && !type.Namespace.Contains("Lists", StringComparison.InvariantCultureIgnoreCase)
                && !type.Namespace.Contains("Commands", StringComparison.InvariantCultureIgnoreCase)
                && !type.Namespace.Contains("Queries", StringComparison.InvariantCultureIgnoreCase)
                && !typeof(IRequest).IsAssignableFrom(type)
                && !typeof(IBaseRequest).IsAssignableFrom(type)
                && !type.IsAssignableTo(typeof(IRequest))
                && !type.IsAssignableTo(typeof(IBaseRequest))
                && type.IsClass
                && !type.IsAbstract
                && !type.IsRecord()
                && type.GetInterfaces().Length > 0))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            // Database Context Register
            _ = services.AddSingleton<AuditableEntityInterceptor>();
            _ = services.AddDbContextPool<ApplicationDbContext>(
                (sp, option) =>
                {
                    // Register the database context
                    _ = option.UseNpgsql(applicationOptions.ConnectionString, options =>
                    {
                        _ = options.CommandTimeout(500);
                    });

                    // Register the auditable entity interceptor
                    _ = option.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());

                    // Get the environment
                    var environment = sp.GetRequiredService<IWebHostEnvironment>();

                    // Log to Debugger in case of any environment except production
                    if (!environment.IsProduction())
                    {
                        _ = option.EnableSensitiveDataLogging();
                        _ = option.LogTo(
                            message => Debug.WriteLine(message),
                            Microsoft.Extensions.Logging.LogLevel.Information);
                    }
                }, poolSize: 256);

            // Remove Hangfire temporarily to be added later after full revamp
            //_ = services.AddHangfire(opt => _ = opt.UseSqlServerStorage(applicationOptions.ConnectionString));
            //_ = services.AddHangfireServer();

            _ = services.AddCors(opt => opt.AddPolicy("AllowLocalhost5281", policy => policy
                .WithOrigins(applicationOptions.Origins?.ToArray() ?? new[] { "http://localhost:5281" })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromHours(1))));
        }

    }
#pragma warning restore CA1506 // Class Coupling

}

// Extension method to check if a type is a record
public static class TypeExtensions
{
    public static bool IsRecord(this Type type)
    {
        return type.GetMethod("<Clone>$") != null;
    }
}
