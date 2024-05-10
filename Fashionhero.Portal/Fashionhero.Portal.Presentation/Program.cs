
namespace Fashionhero.Portal.Presentation
{
    public class Program
    {
        private static ILogger<Program>? Logger;

        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            var startup = new Startup(builder.Configuration);

            startup.SetupServices(builder.Services);
            WebApplication app = builder.Build();
            startup.SetupApplication(app);
            Logger = startup.ServiceProvider.GetService<ILogger<Program>>();
            app.Lifetime.ApplicationStopping.Register(ApplicationStopping);

            app.Run();
        }

        private static void ApplicationStopping()
        {
            Logger?.LogInformation($"Shutting down...{Environment.NewLine}");
        }
    }
}
