using Microsoft.Extensions.DependencyInjection;
using Stickto.Shared.Infrastructure.Options.Application;
using System.Reflection;

namespace Stickto.Shared.Infrastructure.Dependency
{
    /// <summary>
    /// Provides extension methods for installing services into an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Installs services into an <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="applicationOptions">The application options.</param>
        /// <param name="assemblies">The assemblies to scan for services.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection InstallServices(
            this IServiceCollection services,
            ApplicationOptions applicationOptions,
            params Assembly[] assemblies)
        {
            IEnumerable<IServiceInstaller> serviceInstallers = assemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(IsAssignableToType<IServiceInstaller>)
                .Select(Activator.CreateInstance)
                .Cast<IServiceInstaller>();
            foreach (IServiceInstaller serviceInstaller in serviceInstallers)
            {
                serviceInstaller.Install(services, applicationOptions);
            }

            return services;
            static bool IsAssignableToType<T>(TypeInfo typeInfo) =>
                typeof(T).IsAssignableFrom(typeInfo) &&
                !typeInfo.IsInterface &&
                !typeInfo.IsAbstract;
        }
    }
}
