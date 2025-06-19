using WebApi.Services.Http;
using WebApi.Services.Hubs;
using WebApi.Services.Parsers;

namespace WebApi.Services;

public static class WebServicesExtensions
{
    public static void AddWebServices(this IServiceCollection services)
    {
        services.AddScoped<UnitOfWork>();
        services.AddScoped(typeof(UserContext<,>));
        services.AddScoped<EmbeddingService>();
        services.AddScoped<FolderMetaDataService>();
        services.AddSingleton<NoteHubService>();
        services.AddScoped<BlockNoteParserService>();
        services.AddScoped<FolderLlmChoiceService>();
        services.AddScoped<TopicExtractorService>();
        services.AddScoped<MindMapService>();
    }
}