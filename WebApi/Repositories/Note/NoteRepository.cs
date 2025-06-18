using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Generics;
using WebApi.Services;
using WebApi.Services.Parsers;

namespace WebApi.Repositories.Note;

public class NoteRepository : Repository<Data.Entities.Note, Guid>, INoteRepository
{
    private readonly IMapper _mapper;
    private readonly EmbeddingService _embeddingService;
    private readonly BlockNoteParserService _blockNoteParserService;
    
    public NoteRepository(ApplicationDbContext context,
        IMapper mapper,
        EmbeddingService embeddingService,
        BlockNoteParserService blockNoteParserService) : base(context)
    {
        _mapper = mapper;
        _embeddingService = embeddingService;
        _blockNoteParserService = blockNoteParserService;
    }

    public async Task<List<Data.Entities.Note>> GetNotesByNoteIdsAsync(List<Guid> noteIds)
    {
        return await DbSet
            .Where(n => !n.IsDeleted)
            .Where(n => noteIds.Contains(n.Id)).ToListAsync();   
    }

    public async Task<List<NoteDtoVeryMinimal>> SearchNotesForAgent(string userId, Vector searchVector, int top = 5)
    {
        var notes = await DbSet
            .AsNoTracking()
            .Where(n => !n.IsDeleted)
            .Where(n => n.UserId == userId)
            .Where(n => n.Embedding != null)
            .OrderBy(n => n.Embedding!.CosineDistance(searchVector))
            .Skip(0)
            .Take(top)
            .ProjectTo<NoteDtoVeryMinimal>(_mapper.ConfigurationProvider)
            .ToListAsync();
        
        return notes;
    }

    public async Task<List<NoteDtoWithTopics>> SearchNotesForAgentWithTopics(string userId, Vector searchVector,
        int top = 15)
    {
        var notes = await DbSet
            .AsNoTracking()
            .Where(n => !n.IsDeleted)
            .Where(n => n.UserId == userId)
            .Where(n => n.Embedding != null)
            .OrderBy(n => n.Embedding!.CosineDistance(searchVector))
            .Skip(0)
            .Take(top)
            .ProjectTo<NoteDtoWithTopics>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return notes;
    }
    
    public async Task<PaginatedResult<NoteDtoMinimal>> SearchNotesAsync(string userId, string? searchTerm = null,
        int skip = 0, int take = 10)
    {
        var baseQuery 
            = DbSet
                .AsNoTracking()
                .Where(n => !n.IsDeleted)
                .Where(n => n.UserId == userId);
        var totalCount = await baseQuery.CountAsync();


        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchVector = await _embeddingService.GenerateEmbeddings(searchTerm);

            // First try: full-text search
            var textSearchQuery = baseQuery
                .Where(
                    n =>
                        EF.Functions.ToTsVector("english", n.Title).SetWeight(NpgsqlTsVector.Lexeme.Weight.A)
                            .Concat(EF.Functions.ToTsVector("english", n.Summary!).SetWeight(NpgsqlTsVector.Lexeme.Weight.B))
                            .Concat(EF.Functions.ToTsVector("english", n.NormalizedContent!).SetWeight(NpgsqlTsVector.Lexeme.Weight.C))
                        .Matches(EF.Functions.PhraseToTsQuery("english", searchTerm)))
                .OrderBy(n => n.Embedding!.CosineDistance(searchVector))
                .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
                .Skip(skip)
                .Take(take);

            var notes = await textSearchQuery.ToListAsync();

            // If full-text returned nothing, fallback to semantic search only
            if (notes.Count == 0)
            {
                var semanticQuery = baseQuery
                    .OrderBy(n => n.Embedding!.CosineDistance(searchVector))
                    .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
                    .Skip(skip)
                    .Take(take);

                notes = await semanticQuery.ToListAsync();
            }
            
            
            var notesDto = new List<NoteDtoMinimal>();
            if (notes.Count != 0)
            {
                foreach (var note in notes)
                {
                    var noteDto = _mapper.Map<NoteDtoMinimal>(note);
                    noteDto.Snippet = _blockNoteParserService.ExtractSnippet(searchTerm, note);
                    notesDto.Add(noteDto);
                }
            }
            else
            {
                notesDto = _mapper.Map<List<NoteDtoMinimal>>(notes);
            } 
                
            return new PaginatedResult<NoteDtoMinimal>
            {
                Items = notesDto,
                TotalItems = totalCount
            };
        }

        var noteDtoMinimal = await baseQuery
            .OrderByDescending( n => n.UpdatedAt)
            .Skip(skip)
            .Take(take)
            .ProjectTo<NoteDtoMinimal>(_mapper.ConfigurationProvider)
            .ToListAsync();
        
        return new PaginatedResult<NoteDtoMinimal>
        {
            Items = noteDtoMinimal,
            TotalItems = totalCount
        };
    }


    public async Task<PaginatedResult<NoteDtoMinimal>> GetAllNotesForUserAsync(string userId, Vector? searchVector = null, int skip = 0, int take = 10)
    {
        var baseQuery = DbSet.AsNoTracking()
            .Where(n => !n.IsDeleted)
            .Where(n => n.UserId == userId);

        var totalCount = await baseQuery.CountAsync();
        
        if (searchVector != null)
        {
            baseQuery = baseQuery
                .OrderBy(n => n.Embedding!.CosineDistance(searchVector))
                .ThenByDescending(n => n.UpdatedAt);
        }
        else
        {
            baseQuery = baseQuery.OrderByDescending(n => n.UpdatedAt);
        }
        
        var notes =  await baseQuery
            .Skip(skip)
            .Take(take)
            .ProjectTo<NoteDtoMinimal>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PaginatedResult<NoteDtoMinimal>
        {
            Items = notes,
            TotalItems = totalCount,
        };
    }

    public async Task<NoteNormalizedDto?> GetNormalizedNoteContent(Guid noteId)
    {
        return await DbSet
            .Where(n => !n.IsDeleted)
            .Where(n => n.Id == noteId)
            .AsNoTracking()
            .Select(n => new NoteNormalizedDto
            {
                Id = n.Id,
                UserId = n.UserId,
                NormalizedContent = n.NormalizedContent,
                Title = n.Title
            })
            .FirstOrDefaultAsync();
    }

    
    public async Task<NoteDto?> FindNoteWithProjection(Guid noteId)
    {
        return await DbSet
            .Where(n => !n.IsDeleted)
            .Where(n => n.Id == noteId)
            .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public override async Task<Data.Entities.Note?> FindByIdAsync(Guid id)
    {
        return await DbSet
            .Where(n => !n.IsDeleted)
            .Where(n => n.Id == id)
            .Include(n => n.User)
            .Include(n=> n.Folder )
            .FirstOrDefaultAsync();
    }

    public async Task<List<NoteDtoMinimal>> FindDeletedNotesWithProjection(string userId)
    {
        return await DbSet
            .Where(n => n.IsDeleted)
            .Where(n => n.UserId == userId)
            .ProjectTo<NoteDtoMinimal>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Data.Entities.Note?> FindDeletedNote(Guid noteId)
    {
        return await DbSet
            .Where(n => n.IsDeleted)
            .Where(n => n.Id == noteId)
            .Include(n => n.User)
            .Include(n=> n.Folder )
            .FirstOrDefaultAsync();
    }
}