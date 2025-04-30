using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Repositories;

public class NoteRepository : Repository<Note, Guid>
{
    private readonly IMapper _mapper;
    
    public NoteRepository(ApplicationDbContext context, IMapper mapper) : base(context)
    {
        _mapper = mapper;
    }

    public async Task<PaginatedResult<NoteDto>> GetAllNotesForUserAsync(string userId, Vector? searchVector = null, int skip = 0, int take = 10)
    {
        var baseQuery = DbSet.AsNoTracking().Where(n => n.UserId == userId);

        var totalCount = await baseQuery.CountAsync();
        
        if (searchVector != null)
        {
            baseQuery = baseQuery.OrderBy(n => 
                n.Embedding!.CosineDistance(searchVector));
        }
        
        var notes =  await baseQuery
            .Skip(skip)
            .Take(take)
            .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PaginatedResult<NoteDto>
        {
            Items = notes,
            TotalItems = totalCount,
        };
    }

    
    public async Task<NoteDto?> FindNoteWithProjection(Guid noteId)
    {
        return await DbSet.Where(n => n.Id == noteId)
            .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public override async Task<Note?> FindByIdAsync(Guid id)
    {
        return await DbSet.Where(n => n.Id == id)
            .Include(n => n.User)
            .Include(n=> n.Folder )
            .Include(n => n.Keywords)
            .FirstOrDefaultAsync();
    }
}