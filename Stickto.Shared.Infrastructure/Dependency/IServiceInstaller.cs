using Microsoft.Extensions.DependencyInjection;
using Stickto.Shared.Infrastructure.Options.Application;

namespace Stickto.Shared.Infrastructure.Dependency
{
    /// <summary>
    /// Represents a service installer for the application.
    /// This interface defines a method for installing services into an <see cref="IServiceCollection"/>.
    /// </summary>
    public interface IServiceInstaller
    {
        /// <summary>
        /// Installs services into an <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="applicationOptions">The application options setup.</param>
        void Install(IServiceCollection services, ApplicationOptions applicationOptions);
    }
}
