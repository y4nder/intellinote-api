using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Repositories;

public class KeywordRepository : Repository<Keyword, Guid>
{
    private readonly IMapper _mapper;
    
    public KeywordRepository(ApplicationDbContext context, IMapper mapper) : base(context)
    {
        _mapper = mapper;
    }

    public async Task<List<KeywordDto>> GetAllKeywordsWithMappingAsync()
    {
        return await DbSet.ProjectTo<KeywordDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<KeywordDto?> GetKeywordWithMappingAsync(Guid id)
    {
        return await DbSet
            .ProjectTo<KeywordDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Keyword>> GetExistingKeywords(
        List<Guid> keywordIds,
        List<String> keywordNames,
        CancellationToken ct = default)
    {
        // returns a list of keywords that matches the names and the ids
        return await DbSet
            .Where(x => 
                keywordIds.Contains(x.Id) || 
                keywordNames.Contains(x.Name))
            .ToListAsync(ct);
    }
}