using Fashionhero.Portal.BusinessLogic;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Startup;

namespace Fashionhero.Portal.Presentation
{
    public class PortalStartupModule : IStartupModule
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IXmlLoaderService, LoaderService>();
            services.AddScoped<IXmlExportService, SpartooService>();
        }

        /// <inheritdoc />
        public void ConfigureApplication(IApplicationBuilder app)
        {
        }
    }
}