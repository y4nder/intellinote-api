using MediatR;
using Quartz;
using WebApi.Data.Entities;
using WebApi.Features.Notes.Jobs;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.Repositories.Note;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Http;
using WebApi.Services.Parsers;

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
        private readonly INoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly BlockNoteParserService  _blockNoteParserService;     

        public Handler(INoteRepository noteRepository, UserContext<User, string> userContext, ISchedulerFactory schedulerFactory, BlockNoteParserService blockNoteParserService)
        {
            _noteRepository = noteRepository;
            _userContext = userContext;
            _schedulerFactory = schedulerFactory;
            _blockNoteParserService = blockNoteParserService;
        }

        public async Task<Result<SummarizeNoteResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.FindByIdAsync(request.NoteId);
            
            var owned = OwnershipValidator.Ensure(
                note,
                n => n.User.Id == _userContext.Id()
            );
            if (owned.IsFailure) 
                return Result.Failure<SummarizeNoteResponse>(owned.Error!);
            
            var text = _blockNoteParserService.ExtractNoteBlockContents(note!);
            
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            var jobData = new JobDataMap()
            {
                { "noteId", note!.Id.ToString() },
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