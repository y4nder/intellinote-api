using WebApi.Repositories.Folder;
using WebApi.Repositories.Note;
using WebApi.Repositories.UserData;
using WebApi.Repositories.View;

namespace WebApi.Repositories;

public static class RepositoryExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserDataRepository, UserDataRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IFolderRepository, FolderRepository>();
        services.AddScoped<IViewRepository, ViewRepository>();
    }
}