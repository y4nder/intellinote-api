using FluentEmail.Core;
using MediatR;
using WebApi.Generics;

namespace WebApi.Features.SampleFeature.Events;

public class SampleCreatedEvent : DomainEvent, INotification
{
    public string Email { get; set; } = null!;
}

public class SampleCreatedEventHandler : INotificationHandler<SampleCreatedEvent>
{
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<SampleCreatedEventHandler> _logger;

    public SampleCreatedEventHandler(IFluentEmail fluentEmail, ILogger<SampleCreatedEventHandler> logger)
    {
        _fluentEmail = fluentEmail;
        _logger = logger;
    }

    public async Task Handle(SampleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SampleCreatedEvent...");

        var email = _fluentEmail
            .To(notification.Email)
            .Subject("Sample Created")
            .Body($"A new sample has been created");

        var response = await email.SendAsync();
        if (!response.Successful)
        {
            _logger.LogError("Failed to send email: {Errors}", string.Join(", ", response.ErrorMessages));
        }
    }
}
