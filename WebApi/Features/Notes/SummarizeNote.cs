using MediatR;
using Quartz;
using WebApi.Data.Entities;
using WebApi.Features.Notes.Jobs;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Notes;

public class SummarizeNote
{
    public class Command : IRequest<Result<SummarizeNoteResponse>>
    {
        public Guid NoteId { get; set; }
    }

    public class SummarizeNoteResponse
    {
        public string Message { get; set; } = null!;
        public bool Approved { get; set; }
    }
    
    internal sealed class Handler : IRequestHandler<Command, Result<SummarizeNoteResponse>>
    {
        private readonly NoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;
        private readonly ISchedulerFactory _schedulerFactory;

        public Handler(NoteRepository noteRepository, UserContext<User, string> userContext, ISchedulerFactory schedulerFactory)
        {
            _noteRepository = noteRepository;
            _userContext = userContext;
            _schedulerFactory = schedulerFactory;
        }

        public async Task<Result<SummarizeNoteResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.FindNoteWithProjection(request.NoteId);
            
            var owned = OwnershipValidator.Ensure(
                note,
                n => n.Author.Id == _userContext.Id()
            );
            if (owned.IsFailure) 
                return Result.Failure<SummarizeNoteResponse>(owned.Error!);
            
            var text = $"{note!.Title} {note.Content}";
            
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            var jobData = new JobDataMap()
            {
                { "noteId", note.Id.ToString() },
                { "text", text }
            };

            var trigger = TriggerBuilder.Create()
                .ForJob(GenerateKeywordAndSummaryJob.Name)
                .WithIdentity(Guid.NewGuid() + "-noteSummarize")
                .UsingJobData(jobData)
                .StartNow()
                .Build();
            
            await scheduler.ScheduleJob(trigger, cancellationToken);

            return Result.Success(new SummarizeNoteResponse
            {
                Message = "Approved",
                Approved = true
            });
        }
    }
}