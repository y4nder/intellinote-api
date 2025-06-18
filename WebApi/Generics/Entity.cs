namespace WebApi.Generics;

public abstract class Entity<TId>
{
    public TId Id { get; protected init; } = default!;
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    
    protected void SetUpdated() => UpdatedAt = DateTime.UtcNow;
    protected Entity(){}
}