namespace WebApi.Repositories;

public static class RepositoryExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<SampleRepository>();
        services.AddScoped<UserDataRepository>();
        services.AddScoped<NoteRepository>();
        services.AddScoped<FolderRepository>();
        services.AddScoped<KeywordRepository>();
    }
}