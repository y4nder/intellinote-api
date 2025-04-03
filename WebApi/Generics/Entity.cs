namespace WebApi.Generics;

public abstract class Entity<TId> : IEntityWithEvents
{
    public TId Id { get; protected init; } = default(TId)!;

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    protected Entity(){}

    protected void SetUpdated() => UpdatedAt = DateTime.UtcNow;
}