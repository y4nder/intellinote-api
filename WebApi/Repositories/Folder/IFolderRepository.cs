using Microsoft.EntityFrameworkCore;
using Pgvector;
using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Repositories.Folder;

public interface IFolderRepository : IRepository<Data.Entities.Folder, Guid>
{
    DbSet<Data.Entities.Folder> GetDbSet();

    Task<PaginatedResult<FolderWithDetailsDto>> GetFoldersWithDetailsAsync(string userId, Vector? searchVector = null,
        int skip = 0, int take = 10);
    Task<List<FolderWithDetailsDtoMinimal>> GetTopFoldersForAgent(string userId, Vector searchVector, int top = 5);
    Task<List<FolderWithoutDetailsDto>> GetFoldersWithoutDetailsMinimalAsync(string userId, int take = 10);
    Task<FolderWithDetailsDto?> GetFolderWithDetailsAsync(Guid folderId);
    Task<FolderWithDetailsDtoMinimal?> GetFolderWithDetailsMinimalAsync(Guid folderId);
    Task<Data.Entities.Folder?> FindByIdAsyncWithNotes(Guid id);
}