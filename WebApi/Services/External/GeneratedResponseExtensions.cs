namespace WebApi.Services.External;

public static class GeneratedResponseExtensions
{
    public static IServiceCollection AddGeneratedResponseService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<GeneratedResponseService>((sp, client) =>
        {
            var baseUrl = configuration.GetValue<string>("ExternalServices:GeneratedResponse:BaseUrl");
            client.BaseAddress = new Uri(baseUrl!);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        })
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        return services;
    }
}