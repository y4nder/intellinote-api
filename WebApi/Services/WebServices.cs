
namespace WebApi.Services;

public static class WebServices
{
    public static void AddWebServices(this IServiceCollection services)
    {
        services.AddScoped<UnitOfWork>();
        services.AddScoped(typeof(UserContext<,>));
        services.AddScoped<EmbeddingService>();
    }
}