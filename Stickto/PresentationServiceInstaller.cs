using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Stickto.OpenApi;
using Stickto.Shared.Infrastructure.Dependency;
using Stickto.Shared.Infrastructure.Options.Application;

namespace Stickto
{
    /// <summary>
    /// Represents a service installer for the presentation services.
    /// This class is responsible for installing presentation services into
    /// an <see cref="IServiceCollection"/>.
    /// </summary>
    public class PresentationServiceInstaller : IServiceInstaller
    {
        /// <summary>
        /// Installs services into an <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="applicationOptions">The application options setup.</param>
        public void Install(IServiceCollection services, ApplicationOptions applicationOptions)
        {
            _ = services.AddControllers();
            _ = services.AddEndpointsApiExplorer();

            _ = services.AddSwaggerGen();

            // API Versioning Register
            _ = services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc();

            _ = services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            _ = services.ConfigureOptions<ConfigureSwaggerOptions>();
        }
    }
}
