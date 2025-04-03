namespace WebApi.Generics;

public class PaginatedList<TEntity>
{
    public List<TEntity> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

    public PaginatedList(List<TEntity> items, int totalItems, int pageNumber, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
