using Fashionhero.Portal.Shared.Abstraction.Interfaces.Startup;
using Serilog;

namespace Fashionhero.Portal.Presentation.Core
{
    public class LoggingStartupModule : IStartupModule
    {
        //private const string LOGGING_SECTION = "Logging";
        private const string logPattern =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u}] [{SourceContext}] {Message}{NewLine}{Exception}";

        private readonly string logPath;
        //private readonly IConfiguration configuration;

        public LoggingStartupModule(string logPath /*, IConfiguration configuration*/)
        {
            this.logPath = logPath;
            //this.configuration = configuration;
        }

        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console(outputTemplate: logPattern)
                .WriteTo.File(logPath, outputTemplate: logPattern, shared: true,
                    flushToDiskInterval: TimeSpan.FromMinutes(1)).CreateLogger();

            services.AddLogging(x => x.AddSerilog(Log.Logger));

            var logger = services.BuildServiceProvider().GetService<ILogger<LoggingStartupModule>>();
            logger?.Log(LogLevel.Information, "Completed setup of 'Serilog'");
        }

        //private LogLevel GetConfiguredLogLevel()
        //{
        //    IConfigurationSection loggingSection = configuration.GetSection(LOGGING_SECTION);

        //    if (!loggingSection.Exists() || loggingSection.Value == null)
        //        throw new ArgumentException($"Missing '{LOGGING_SECTION}' section in appsettings.json file.");

        //    var level = JsonConvert.DeserializeObject<Level>(loggingSection.Value);
        //    var rawLogLevel = level?.Default.ToLowerInvariant() 
        //                      ?? throw new ArgumentException($"Failed to deserialize logging section to a valid object.");

        //    return rawLogLevel switch
        //    {
        //        "trace" => LogLevel.Trace,
        //        "debug" => LogLevel.Debug,
        //        "information" => LogLevel.Information,

        //        _ => throw new ArgumentException($"Unknown Log Level detected: {rawLogLevel}")
        //    };
        //}

        /// <inheritdoc />
        public void ConfigureApplication(IApplicationBuilder app)
        {
        }

        //private class Level
        //{
        //    public string Default { get; set; }

        //    [JsonProperty("Microsoft.AspNetCore")] public string MicrosoftAspNetCore { get; set; }
        //}

        //private class Root
        //{
        //    public LogLevel LogLevel { get; set; }
        //}
    }
}