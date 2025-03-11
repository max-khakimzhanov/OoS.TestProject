namespace OoS.TestProject.WebApi.Extensions
{
    public static class LoggingConfiguration
    {
        public static void AddLoggingServices(this IServiceCollection services)
        {
            services.AddLogging(options =>
            {
                options.AddConsole();
                options.AddDebug();
            });
        }
    }
}
