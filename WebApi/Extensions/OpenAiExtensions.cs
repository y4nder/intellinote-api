using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using OpenAI.Embeddings;
#pragma warning disable OPENAI001

namespace WebApi.Extensions;

public static class OpenAiExtensions
{
    public static IServiceCollection AddOpenAiExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenAiSettings>(
            configuration.GetSection(nameof(OpenAiSettings))
        );
        
        services.AddScoped<EmbeddingClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<OpenAiSettings>>();
            return new EmbeddingClient(settings.Value.EmbeddingModel, settings.Value.ApiKey);
        });

        services.AddScoped<ChatClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<OpenAiSettings>>();
            return new ChatClient(settings.Value.DescriptionGeneratorModel, settings.Value.ApiKey);
        });
        
        services.AddScoped<OpenAIClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<OpenAiSettings>>();
            return new OpenAIClient(settings.Value.ApiKey);

        });
        
        services.AddScoped<AssistantClient>(sp =>
        {
            var client = sp.GetRequiredService<OpenAIClient>();
            return client.GetAssistantClient();
        }); 
        
        return services;
    }
}

public class OpenAiSettings
{
    public string ApiKey { get; init; } = null!;
    public string EmbeddingModel { get; init; } = null!;
    public string DescriptionGeneratorModel { get; init; } = null!;
    public string AssistantId { get; init; } = null!;
}