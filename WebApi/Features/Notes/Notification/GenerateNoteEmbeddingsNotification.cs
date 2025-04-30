using MediatR;
using Quartz;
using WebApi.Features.Notes.Jobs;

namespace WebApi.Features.Notes.Notification;

public class GenerateNoteEmbeddingsNotification : INotification
{
    public Guid NoteId { get; set; }
    public String TextToEmbed { get; set; } = null!;
}

public class GenerateNoteEmbeddingsNotificationHandler : INotificationHandler<GenerateNoteEmbeddingsNotification>
{
    
    private readonly ISchedulerFactory _schedulerFactory;

    public GenerateNoteEmbeddingsNotificationHandler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task Handle(GenerateNoteEmbeddingsNotification notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.TextToEmbed)) return;

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var jobData = new JobDataMap()
        {
            { "noteId", notification.NoteId.ToString() },
            { "textToEmbed", notification.TextToEmbed }
        };
        
        var trigger = TriggerBuilder.Create()
            .ForJob(GenerateNoteEmbeddings.Name)
            .WithIdentity(Guid.NewGuid() + "-noteEmbeddings")
            .UsingJobData(jobData)
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(trigger, cancellationToken);
    }
}