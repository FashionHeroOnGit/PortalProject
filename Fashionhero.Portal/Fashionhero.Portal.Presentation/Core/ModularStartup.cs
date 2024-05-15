using Fashionhero.Portal.Shared.Abstraction.Interfaces.Startup;

namespace Fashionhero.Portal.Presentation.Core
{
    public abstract class ModularStartup : IStartupModule
    {
        private readonly IList<IStartupModule> _modules;
        public IConfiguration Configuration { get; private set; }
        public IServiceCollection Services { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }


        protected ModularStartup(IConfiguration config)
        {
            Configuration = config ?? throw new ArgumentNullException(nameof(config));
            _modules = new List<IStartupModule>();
        }

        protected ModularStartup()
        {
            const string filename = "appsettings.json";
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), filename)))
                throw new FileNotFoundException(
                    $"Missing {filename} in main directory at {Path.Combine(Directory.GetCurrentDirectory(), filename)}");

            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(filename, false, true);

            Configuration = builder.Build();
            _modules = new List<IStartupModule>();
        }


        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            SetupServices(services);
        }

        /// <inheritdoc />
        public virtual void ConfigureApplication(IApplicationBuilder app)
        {
            SetupApplication(app);
        }

        protected void AddModule(IStartupModule module)
        {
            _modules.Add(module);
        }

        public void SetupServices(IServiceCollection? services = null)
        {
            Services = services ??= new ServiceCollection();

            foreach (IStartupModule module in _modules)
                module.ConfigureServices(Services);

            ServiceProvider = Services.BuildServiceProvider();
        }

        public void SetupApplication(IApplicationBuilder? app = null)
        {
            app ??= new ApplicationBuilder(ServiceProvider);

            foreach (IStartupModule module in _modules)
                module.ConfigureApplication(app);
        }
    }
}