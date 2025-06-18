using AutoMapper;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Features.Notes.Notification;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.Repositories.Note;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Parsers;

namespace WebApi.Features.Notes;

public class UpdateNote
{
    public class Command : IRequest<Result<NotesContracts.UpdateNoteResponse>>
    {
        public Guid NoteId { get; set; }
        public String? Title { get; set; }
        public String? Content { get; set; }
        public String? Summary { get; set; }
    }
    
    internal sealed class Handler  : IRequestHandler<Command, Result<NotesContracts.UpdateNoteResponse>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserContext<User, string> _userContext;
        private readonly IMediator _mediator;
        private readonly BlockNoteParserService _blockNoteParserService;
        private readonly IMapper _mapper;

        public Handler(INoteRepository noteRepository,
            UnitOfWork unitOfWork,
            IMapper mapper,
            UserContext<User, string> userContext,
            IMediator mediator,
            BlockNoteParserService blockNoteParserService)
        {
            _noteRepository = noteRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext = userContext;
            _mediator = mediator;
            _blockNoteParserService = blockNoteParserService;
        }

        public async Task<Result<NotesContracts.UpdateNoteResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingNote = await _noteRepository.FindByIdAsync(request.NoteId);

            var owned = OwnershipValidator.Ensure(existingNote, e => e.UserId == _userContext.Id());

            if (owned.IsFailure) return Result.Failure<NotesContracts.UpdateNoteResponse>(owned.Error!);
            
            existingNote!.Update(new Note
            {
                Title = request.Title ?? existingNote.Title,
                Content = request.Content ?? existingNote.Content,
                Summary = request.Summary ?? existingNote.Summary
            });
            
            var saved = await _unitOfWork.Commit(cancellationToken);
            if(saved.IsFailure) return Result.Failure<NotesContracts.UpdateNoteResponse>(saved.Error!);

            var noteDto = _mapper.Map<NoteDto>(existingNote);
            if (!string.IsNullOrWhiteSpace(existingNote.Content))
            {
                await HandleEmbeddings(existingNote, cancellationToken);
            }
            
            return Result.Success(new NotesContracts.UpdateNoteResponse(noteDto));
        }
        
        private async Task HandleEmbeddings(Note note, CancellationToken cancellationToken)
        {
            var textToEmbed = _blockNoteParserService.PrepareNoteForEmbedding(note);
            if(string.IsNullOrEmpty(textToEmbed)) return;
            await _mediator.Publish(new GenerateNoteEmbeddingsNotification
            {
                NoteId = note.Id,
                TextToEmbed = textToEmbed
            }, cancellationToken);
        }
    }
}