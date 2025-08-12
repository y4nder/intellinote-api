namespace WebApi.Services.External;

public static class GeneratedResponseExtensions
{
    public static void AddGeneratedResponseService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<GeneratedResponseService>((_, client) =>
        {
            var baseUrl = configuration.GetValue<string>("ExternalServices:GeneratedResponse:BaseUrl");
            client.BaseAddress = new Uri(baseUrl!);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        })
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
    }
}