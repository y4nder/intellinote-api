using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Repositories;

public class ViewRepository : Repository<View, Guid>
{
    private readonly IMapper _mapper;
    public ViewRepository(ApplicationDbContext context, IMapper mapper) : base(context)
    {
        _mapper = mapper;
    }

    public async Task<List<ViewResponseDto>> GetViewsByUserId(string userId)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.UpdatedAt)
            .ThenByDescending(x => x.CreatedAt)
            .ProjectTo<ViewResponseDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ViewResponseDto?> GetViewDtoByIdAsync(Guid viewId, string userId)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => 
                x.Id == viewId && x.UserId == userId 
            )
            .ProjectTo<ViewResponseDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<View?> GetViewByIdAsync(Guid viewId, string userId)
    {   
        return await DbSet
            .Where(x => x.Id == viewId && x.UserId == userId)
            .FirstOrDefaultAsync();
    }
}