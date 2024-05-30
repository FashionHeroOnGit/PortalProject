using Fashionhero.Portal.Shared.Abstraction.Interfaces.Startup;
using Newtonsoft.Json.Converters;

namespace Fashionhero.Portal.Presentation.Core
{
    public class ApiStartupModule : IStartupModule
    {
        private ILogger<ApiStartupModule>? logger;

        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            logger = services.BuildServiceProvider().GetService<ILogger<ApiStartupModule>>();

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddEndpointsApiExplorer();

            logger?.LogDebug("Completed Configuration of Services.");
        }

        /// <inheritdoc />
        public void ConfigureApplication(IApplicationBuilder app)
        {
            if (app.GetType().FullName != typeof(WebApplication).FullName)
                throw new InvalidOperationException(
                    $"Expected application builder supplied to {nameof(ApiStartupModule)}.{nameof(ConfigureApplication)} to be of type {nameof(WebApplication)}, but it was of type '{app.GetType().FullName}'");

            app.UseHttpsRedirection();
            app.UseAuthorization();

            var castApp = (WebApplication) app;
            castApp.MapControllers();

            logger?.LogDebug("Completed Configuration of Application.");
        }
    }
}