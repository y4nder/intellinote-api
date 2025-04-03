using MediatR;
using Newtonsoft.Json;
using Quartz;
using WebApi.Data.Entities;
using WebApi.Features.Keywords.Jobs;

namespace WebApi.Features.Keywords;

public class CreateBatchKeywordsNotification : INotification
{
    public Guid NoteId { get; set; }
    public List<CreateKeywordDto> Keywords { get; set; } = null!;
}

public class CreateBatchKeywordsNotificationHandler : INotificationHandler<CreateBatchKeywordsNotification>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public CreateBatchKeywordsNotificationHandler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task Handle(CreateBatchKeywordsNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Keywords.Count == 0)
        {
            return;
        }
        
        IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        JobDataMap jobData = new JobDataMap()
        {
            { "keywords", JsonConvert.SerializeObject(notification.Keywords) },
            { "noteId", notification.NoteId.ToString()}
        };
        
        ITrigger trigger = TriggerBuilder.Create()
            .ForJob(BatchInsertNewKeywords.Name )
            .WithIdentity(Guid.NewGuid() + "-keywordBatch")
            .UsingJobData(jobData)
            .StartNow()
            .Build();
        
        await scheduler.ScheduleJob(trigger, cancellationToken);
    }
}