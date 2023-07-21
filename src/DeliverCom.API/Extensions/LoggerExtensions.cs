using Serilog;
using Serilog.Configuration;

namespace DeliverCom.API.Extensions
{
    public static class LoggerExtensions
    {
        public static void AddLogger(this IServiceCollection services,
            Action<IConfigurationBuilder> loggingConfigurationBuilder)
        {
            var configurationBuilder = new ConfigurationBuilder();
            loggingConfigurationBuilder(configurationBuilder);
            var configuration = configurationBuilder.Build();
            AddLogger(services, configuration);
        }

        public static void AddLogger(this IServiceCollection services,
            IConfiguration configuration)
        {
            AddLoggerInternal(services, configuration, null);
        }

        private static void AddLoggerInternal(IServiceCollection services,
            IConfiguration configuration, Action<LoggerEnrichmentConfiguration> enricherConfiguration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(SwitchableLogger.Instance);
                loggingBuilder.AddSerilogConfigurationLoader(configuration,
                    SwitchableLogger.Instance,
                    _ =>
                    {
                        var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration);
                        enricherConfiguration?.Invoke(logger.Enrich);

                        logger.Enrich.FromLogContext();
                        var l = logger.CreateLogger();
                        return l;
                    });
            });
        }
    }
}