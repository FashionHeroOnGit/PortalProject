using Fashionhero.Portal.Shared.Abstraction.Interfaces.Startup;
using Microsoft.OpenApi.Models;

namespace Fashionhero.Portal.Presentation.Core
{
    public class SwaggerStartupModule : IStartupModule
    {
        private readonly string apiTitle;

        public SwaggerStartupModule(string apiTitle = "My API")
        {
            this.apiTitle = apiTitle;
        }

        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents.
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = apiTitle, Version = "v1",}); });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        /// <inheritdoc />
        public void ConfigureApplication(IApplicationBuilder app)
        {
            if (app.GetType().FullName != typeof(WebApplication).FullName)
                throw new InvalidOperationException(
                    $"Expected application builder supplied to {nameof(SwaggerStartupModule)}.{nameof(ConfigureApplication)} to be of type {nameof(WebApplication)}, but it was of type '{app.GetType().FullName}'");

            var castApp = (WebApplication) app;
            if (!castApp.Environment.IsDevelopment())
                return;

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiTitle}");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}