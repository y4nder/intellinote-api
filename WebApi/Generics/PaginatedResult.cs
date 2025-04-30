namespace WebApi.Generics;

public class PaginatedResult<TEntity>
{
    public List<TEntity> Items { get; set; } = new();
    public int TotalItems { get; set; }
}
