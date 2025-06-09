using Microsoft.EntityFrameworkCore;

namespace WebApi.Generics;

// Author's note: Don't forget to register inheriting classes in the Service Extensions

public interface IRepository<TEntity, in TIdentifier> where TEntity : Entity<TIdentifier>
{
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<TEntity?> FindByIdAsync(TIdentifier id);
    Task<List<TEntity>> GetAllAsync();
}

public abstract class Repository<TEntity, TIdentifier> : IRepository<TEntity, TIdentifier> where TEntity : Entity<TIdentifier>
{
    protected readonly DbSet<TEntity> DbSet;
    
    protected Repository(DbContext context)
    {
        DbSet = context.Set<TEntity>();
    }
    
    public void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }
    
    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public virtual async Task<TEntity?> FindByIdAsync(TIdentifier id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }
    
    // public async Task<PaginatedResult<TEntity>> GetFilteredPaginatedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? expression = null )
    // {
    //     IQueryable<TEntity> filteredQuery = DbSet.AsQueryable();
    //
    //     if (expression is not null)
    //         filteredQuery = filteredQuery.Where(expression);
    //
    //     var totalItems = await filteredQuery.CountAsync();
    //     var items = await filteredQuery
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToListAsync();
    //
    //     return new PaginatedResult<TEntity>(items, totalItems, pageNumber, pageSize);
    // }
}