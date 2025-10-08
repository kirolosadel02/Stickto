using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Stickto.Shared.Infrastructure.Configurations.Authentication
{
    /// <summary>
    /// This class is responsible for configuring the options for cookie authentication.
    /// </summary>
    internal sealed class ConfigureCookie : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        /// <summary>
        /// Configures the provided <see cref="CookieAuthenticationOptions"/>.
        /// </summary>
        /// <param name="options">The options to configure.</param>
        public void Configure(CookieAuthenticationOptions options)
        {
            options.AccessDeniedPath = "/AccessDenied";
            options.Cookie.Name = "Cookie";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(720);
            options.LoginPath = "/auth/login";
            options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            options.SlidingExpiration = true;
        }

        /// <summary>
        /// Configures the provided <see cref="CookieAuthenticationOptions"/> for a specific scheme.
        /// </summary>
        /// <param name="name">The name of the authentication scheme to configure.</param>
        /// <param name="options">The options to configure.</param>
        public void Configure(string name, CookieAuthenticationOptions options)
        {
            Configure(options);
        }
    }
}
