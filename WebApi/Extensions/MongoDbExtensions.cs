using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace WebApi.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDbExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        // bind settings from app settings
        services.Configure<MongoDbSettings>(
            configuration.GetSection(nameof(MongoDbSettings))
        );

        // add singleton for MongoDB client
        services.AddSingleton<MongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>();
            return new MongoClient(settings.Value.ConnectionString);
        });
        
        return services;
    }
}

public class MongoDbSettings
{
    public String ConnectionString { get; set; } = null!;
    public String DatabaseName { get; set; } = null!;
}