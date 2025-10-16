using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Stickto.Shared.Infrastructure.Options.Application;

namespace Stickto.Shared.Infrastructure.Configurations.Authentication
{
    /// <summary>
    /// Configures Google OAuth authentication
    /// </summary>
    internal sealed class ConfigureGoogleAuth : IConfigureNamedOptions<GoogleOptions>
    {
        private readonly ApplicationOptions _applicationOptions;

        public ConfigureGoogleAuth(IOptionsMonitor<ApplicationOptions> options)
        {
            _applicationOptions = options.CurrentValue;
        }

        public void Configure(GoogleOptions options)
        {
            options.ClientId = _applicationOptions.GoogleAuth?.ClientId ?? string.Empty;
            options.ClientSecret = _applicationOptions.GoogleAuth?.ClientSecret ?? string.Empty;
            options.SaveTokens = true;
        }

        public void Configure(string? name, GoogleOptions options)
        {
            Configure(options);
        }
    }
}

