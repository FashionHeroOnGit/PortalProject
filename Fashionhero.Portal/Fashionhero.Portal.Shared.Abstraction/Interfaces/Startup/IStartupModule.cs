using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Startup
{
    public interface IStartupModule
    {
        /// <summary>
        ///     To be called during call to 'SetupServices', wherein different services are configured.
        /// </summary>
        /// <param name="services"></param>
        void ConfigureServices(IServiceCollection services);

        /// <summary>
        ///     To be called during call to 'SetupApplication', wherein the application is configured.
        ///     'IApplicationBuilder' comes from a deprecated nuget package.
        ///     Todo: Find a non-deprecated source for the type.
        /// </summary>
        /// <param name="app"></param>
        void ConfigureApplication(IApplicationBuilder app);
    }
}