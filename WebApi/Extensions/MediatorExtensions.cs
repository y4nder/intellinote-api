using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Generics;

namespace WebApi.Extensions;

/// <summary>
/// Provides extension methods for dispatching domain events using MediatR.
/// </summary>
public static class MediatorExtensions
{
    /// <summary>
    /// Dispatches all domain events from tracked entities in the DbContext.
    /// </summary>
    /// <param name="mediator">The MediatR instance used for publishing domain events.</param>
    /// <param name="context">The database context tracking the entities.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task DispatchDomainEvents(this IMediator mediator, DbContext context)
    {
        // Retrieve all entities that have domain events
        var entitiesWithEvents = context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IEntityWithEvents entity && entity.DomainEvents.Any())
            .Select(e => (IEntityWithEvents)e.Entity)
            .ToList();

        // Extract all domain events from these entities
        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Clear domain events after extracting them to avoid duplicate processing
        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        // Publish each domain event using MediatR
        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}