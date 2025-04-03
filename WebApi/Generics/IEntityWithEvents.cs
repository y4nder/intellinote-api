namespace WebApi.Generics;

public interface IEntityWithEvents
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}