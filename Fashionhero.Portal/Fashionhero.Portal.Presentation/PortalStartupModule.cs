using Fashionhero.Portal.BusinessLogic;
using Fashionhero.Portal.BusinessLogic.Services;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Startup;
using Jakubqwe.CurrencyConverter;
using Jakubqwe.CurrencyConverter.CurrencyRateProviders;

namespace Fashionhero.Portal.Presentation
{
    public class PortalStartupModule : IStartupModule
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IXmlLoaderService, LoaderService>();
            services.AddScoped<IXmlExportService, SpartooService>();
            services.AddScoped<ICurrencyConverterService, CurrencyConverterService>();
            services.AddScoped<ICurrencyRateProvider, EcbCurrencyProvider>();
        }

        /// <inheritdoc />
        public void ConfigureApplication(IApplicationBuilder app)
        {
        }
    }
}