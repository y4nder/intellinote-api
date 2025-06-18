using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Repositories.Folder;

public class FolderRepository : Repository<Data.Entities.Folder, Guid>, IFolderRepository
{
    private readonly IMapper _mapper;    
    private readonly DbSet<Data.Entities.Folder> _dbSet;
    
    public FolderRepository(ApplicationDbContext context, IMapper mapper) : base(context)
    {
        _mapper = mapper;
        _dbSet = context.Set<Data.Entities.Folder>();
    }

    public DbSet<Data.Entities.Folder> GetDbSet() => _dbSet;

    public async Task<PaginatedResult<FolderWithDetailsDto>> GetFoldersWithDetailsAsync(string userId, Vector? searchVector = null, int skip = 0, int take = 10)
    {
        var baseQuery = DbSet.AsNoTracking()
            .Where(f => f.UserId == userId)
            .Where((f => f.Notes.Any(n => n.IsDeleted == false) || f.Notes.Count == 0));
        
        var totalCount = await baseQuery.CountAsync();

        if (searchVector != null)
        {
            baseQuery = baseQuery
                .Where(f => f.Embedding != null)
                .OrderBy(f => f.Embedding!.CosineDistance(searchVector));
        }
        
        var folders = await baseQuery
            .Skip(skip)
            .Take(take)
            .ProjectTo<FolderWithDetailsDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        
        return new PaginatedResult<FolderWithDetailsDto>
        {
            Items = folders,
            TotalItems = totalCount,
        };
    }

    public async Task<List<FolderWithDetailsDtoMinimal>> GetTopFoldersForAgent(string userId, Vector searchVector, int top = 5)
    {
        return await DbSet.Where(f => f.Embedding != null)
            .Where(f => f.UserId == userId)
            .Where((f => f.Notes.Any(n => n.IsDeleted == false) || f.Notes.Count == 0))
            .Where(f => f.Embedding != null)
            .OrderBy(f => f.Embedding!.CosineDistance(searchVector))
            .Skip(0)
            .Take(top)
            .ProjectTo<FolderWithDetailsDtoMinimal>(_mapper.ConfigurationProvider)
            .ToListAsync();   
    }
    
    public async Task<List<FolderWithoutDetailsDto>> GetFoldersWithoutDetailsMinimalAsync(string userId, int take = 10)
    {
        return await DbSet.AsNoTracking()
            .Where(f => f.UserId == userId)
            .Where((f => f.Notes.Any(n => n.IsDeleted == false) || f.Notes.Count == 0))
            .OrderByDescending(f => f.UpdatedAt)
            .Skip(0)
            .Take(take)
            .ProjectTo<FolderWithoutDetailsDto>(_mapper.ConfigurationProvider)
            .ToListAsync();  
        
    }

    public async Task<FolderWithDetailsDto?> GetFolderWithDetailsAsync(Guid folderId)
    {
        var folder = await DbSet.Where(f => f.Id == folderId)
            .Where((f => f.Notes.Any(n => n.IsDeleted == false) || f.Notes.Count == 0))
            .ProjectTo<FolderWithDetailsDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

        return folder;
    }

    public async Task<FolderWithDetailsDtoMinimal?> GetFolderWithDetailsMinimalAsync(Guid folderId)
    {
        var folder = await DbSet.Where(f => f.Id == folderId)
            .Where((f => f.Notes.Any(n => n.IsDeleted == false) || f.Notes.Count == 0))
            .ProjectTo<FolderWithDetailsDtoMinimal>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        return folder;
    }

    public override async Task<Data.Entities.Folder?> FindByIdAsync(Guid id)
    {
        return await DbSet
            .Where(f => f.Id == id)
            .Where((f => f.Notes.Any(n => n.IsDeleted == false) || f.Notes.Count == 0))
            .Include(f => f.User)
            .FirstOrDefaultAsync();
    }

    public async Task<Data.Entities.Folder?> FindByIdAsyncWithNotes(Guid id)
    {
        return await DbSet.Where(f => f.Id == id)
            .Where((f => f.Notes.Any(n => n.IsDeleted == false) || f.Notes.Count == 0))
            .Include(f => f.User)
            .Include(f => f.Notes)
            .FirstOrDefaultAsync();   
    }   
}