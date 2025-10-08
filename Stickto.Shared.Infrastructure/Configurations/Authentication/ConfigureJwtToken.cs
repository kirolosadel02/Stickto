using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stickto.Shared.Infrastructure.Options.Application;
using System.Text;

namespace Stickto.Shared.Infrastructure.Configurations.Authentication
{
    /// <summary>
    /// This class is responsible for configuring the options for JWT (JSON Web Token) bearer authentication.
    /// </summary>
    internal sealed class ConfigureJwtToken : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly ApplicationOptions _applicationOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureJwtToken"/> class.
        /// </summary>
        /// <param name="options">The application options to use for configuration.</param>
        public ConfigureJwtToken(IOptionsMonitor<ApplicationOptions> options)
        {
            _applicationOptions = options.CurrentValue;
        }

        /// <summary>
        /// Configures the provided <see cref="JwtBearerOptions"/>.
        /// </summary>
        /// <param name="options">The options to configure.</param>
        public void Configure(JwtBearerOptions options)
        {
#pragma warning disable CA5404 // CA5404: Do not use 'Encoding.UTF8.GetBytes' to convert a string to a byte array
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_applicationOptions.Token.Key)),
                ValidateIssuer = false,
                ValidIssuer = _applicationOptions.Token.Issuer,
                ValidAudiences = _applicationOptions.Origins,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
            };
        }

#pragma warning restore CA5404 // CA5404: Do not use 'Encoding.UTF8.GetBytes' to convert a string to a byte array
        /// <summary>
        /// Configures the provided <see cref="JwtBearerOptions"/> for a specific scheme.
        /// </summary>
        /// <param name="name">The name of the authentication scheme to configure.</param>
        /// <param name="options">The options to configure.</param>
        public void Configure(string name, JwtBearerOptions options)
        {
            Configure(options);
        }
    }

}
