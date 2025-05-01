using MediatR;
using Quartz;
using WebApi.Data.Entities;
using WebApi.Features.Folders.Jobs;

namespace WebApi.Features.Folders.Notifications;

public class DelegateFolderEmbeddingNotification : INotification 
{
    public Guid FolderId { get; set; }
    public bool Auto { get; set; }

    public List<Note> Notes { get; set; } = new List<Note>();
    
    public static DelegateFolderEmbeddingNotification
        CreateFolderJob(Guid folderId, List<Note> notes)
    {
        return new DelegateFolderEmbeddingNotification
        {
            FolderId = folderId,
            Auto = true,
            Notes = notes
        };       
    }
    
    public static DelegateFolderEmbeddingNotification
        UpdateFolderJob(Guid folderId, List<Note> notes)
    {
        return new DelegateFolderEmbeddingNotification
        {
            FolderId = folderId,
            Auto = false,
            Notes = notes
        };       
    }
}

public class DelegateFolderEmbeddingNotificationHandler : INotificationHandler<DelegateFolderEmbeddingNotification>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public DelegateFolderEmbeddingNotificationHandler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task Handle(DelegateFolderEmbeddingNotification notification, CancellationToken cancellationToken)
    {
        if (notification.FolderId == Guid.Empty) return;
        if(notification.Notes.Count == 0) return;
        
        var allTextAndTopics = PrepareTexts(notification.Notes);
        
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var jobData = new JobDataMap()
        {
            {"folderId", notification.FolderId.ToString() },
            {"auto", notification.Auto.ToString()},
            {"descriptionToEmbed", allTextAndTopics}
        };

        var trigger = TriggerBuilder.Create()
            .ForJob(GenerateFolderEmbeddings.Name)
            .WithIdentity(Guid.NewGuid() + "-folderEmbeddings")
            .UsingJobData(jobData)
            .StartNow()
            .Build();
            
        await scheduler.ScheduleJob(trigger, CancellationToken.None);        
    }
    
    private string PrepareTexts(List<Note> notes)
    {
        var texts = notes.Select(x => x.FlattenNoteForEmbedding()).ToList();
        var allTextAndTopics = string.Join(" ", texts);
        return allTextAndTopics;
    }
}