using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<FolderWithDetailsDto>> GetFoldersWithDetailsAsync(string userId)
    {
        var folders = await DbSet.Where(f => f.UserId == userId)
            .ProjectTo<FolderWithDetailsDto>(_mapper.ConfigurationProvider).ToListAsync();
        
        return folders;
    }

    public async Task<FolderWithDetailsDto?> GetFolderWithDetailsAsync(Guid folderId)
    {
        var folder = await DbSet.Where(f => f.Id == folderId)
            .ProjectTo<FolderWithDetailsDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

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