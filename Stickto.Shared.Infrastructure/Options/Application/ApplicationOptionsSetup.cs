using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Stickto.Shared.Infrastructure.Options.Application
{
    /// <summary>
    /// Represents a setup for the application options.
    /// This class is responsible for configuring the <see cref="ApplicationOptions"/>.
    /// </summary>
    public class ApplicationOptionsSetup : IConfigureOptions<ApplicationOptions>
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationOptionsSetup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public ApplicationOptionsSetup(IConfiguration configuration) => this.configuration = configuration;

        /// <summary>
        /// Configures the <see cref="ApplicationOptions"/>.
        /// </summary>
        /// <param name="options">The options to configure.</param>
        public void Configure(ApplicationOptions options)
        {
            ConfigurationBinder.Bind(configuration.GetSection("ApplicationSettings"), options);
        }
    }
}
