namespace StockPulse.API.Helpers
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigureSettings<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class
        {
            return services.Configure<T>(configuration.GetSection(typeof(T).Name));
        }

        public static T GetSettings<T>(this IConfiguration configuration) where T : class, new()
        {
            var section = configuration.GetSection(typeof(T).Name);
            return section.Get<T>() ?? new T();
        }
    }
}
