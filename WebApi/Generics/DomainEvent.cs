namespace WebApi.Generics;

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; protected init; } = DateTime.UtcNow;
}

