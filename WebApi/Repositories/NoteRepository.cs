using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<NoteDto>> GetAllNotesForUserAsync(string userId)
    {
        return await DbSet.AsNoTracking()
            .Where(n => n.UserId == userId)
            .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
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