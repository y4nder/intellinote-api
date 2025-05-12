using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Repositories;

public class FolderRepository : Repository<Folder, Guid>
{
    private readonly IMapper _mapper;    
    
    public FolderRepository(ApplicationDbContext context, IMapper mapper) : base(context)
    {
        _mapper = mapper;
    }

    public async Task<PaginatedResult<FolderWithDetailsDto>> GetFoldersWithDetailsAsync(string userId, Vector? searchVector = null, int skip = 0, int take = 10)
    {
        var baseQuery = DbSet.AsNoTracking().Where(f => f.UserId == userId);
        
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

    public async Task<FolderWithDetailsDto?> GetFolderWithDetailsAsync(Guid folderId)
    {
        var folder = await DbSet.Where(f => f.Id == folderId)
            .ProjectTo<FolderWithDetailsDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

        return folder;
    }

    public async Task<FolderWithDetailsDtoMinimal?> GetFolderWithDetailsMinimalAsync(Guid folderId)
    {
        var folder = await DbSet.Where(f => f.Id == folderId)
            .ProjectTo<FolderWithDetailsDtoMinimal>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        return folder;
    }

    public override async Task<Folder?> FindByIdAsync(Guid id)
    {
        return await DbSet.Where(f => f.Id == id)
            .Include(f => f.User)
            .FirstOrDefaultAsync();
    }

    public async Task<Folder?> FindByIdAsyncWithNotes(Guid id)
    {
        return await DbSet.Where(f => f.Id == id)
            .Include(f => f.User)
            .Include(f => f.Notes)
            .FirstOrDefaultAsync();   
    }   
}