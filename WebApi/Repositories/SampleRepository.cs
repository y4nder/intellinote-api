using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Repositories;

public class SampleRepository : Repository<SampleEntity, Guid>
{
    public SampleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<SampleEntity?> GetSampleEntityAsync(String uniqueName)
    {
        var sample =  await DbSet.Where(s => s.UniqueName == uniqueName).SingleOrDefaultAsync();
        return sample;
    }

    public async Task<SampleEntity?> GetSampleEntityByEmail(string email)
    {
        var sample = await DbSet.Where(s => s.Email == email).SingleOrDefaultAsync();
        return sample;
    }
    
    // public async Task<PaginatedList<SampleEntity>> GetPaginatedSamples(int pageIndex, int pageSize)
    // {
    //     var samples = await DbSet.OrderBy(s => s.Id)
    //         .Skip((pageIndex - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToListAsync();
    //     var count = await DbSet.CountAsync();
    //     var totalPages = (int)Math.Ceiling((double)count / pageSize);
    //     
    //     return new PaginatedList<SampleEntity>(samples, pageIndex, totalPages);
    // }
}