
namespace Fashionhero.Portal.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Creating Startup configuration...");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            var startup = new Startup(builder.Configuration);

            startup.SetupServices(builder.Services);
            WebApplication app = builder.Build();
            startup.SetupApplication(app);

            Console.WriteLine("Starting App...");
            app.Run();
        }
    }
}
