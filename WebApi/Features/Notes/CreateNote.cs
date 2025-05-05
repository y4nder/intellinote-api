using AutoMapper;
using FluentValidation;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Features.Notes.Notification;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Parsers;

namespace WebApi.Features.Notes;

public class CreateNote
{
    public class Command : IRequest<Result<NotesContracts.CreateNoteResponse>>
    {
        public String Title { get; set; } = String.Empty;
        public String Content { get; set; } = String.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            // RuleFor(x => x.Title).NotEmpty();
        }
    }
    
    internal sealed class Handler : IRequestHandler<Command, Result<NotesContracts.CreateNoteResponse>>
    {
        private readonly UserContext<User, string> _userContext;
        private readonly NoteRepository _noteRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly BlockNoteParserService _blockNoteParserService;

        public Handler(UserContext<User, string> userContext, NoteRepository noteRepository, UnitOfWork unitOfWork, IMapper mapper, IMediator mediator, BlockNoteParserService blockNoteParserService)
        {
            _userContext = userContext;
            _noteRepository = noteRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
            _blockNoteParserService = blockNoteParserService;
        }

        public async Task<Result<NotesContracts.CreateNoteResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userContext.GetCurrentUser();
            
            var note = Note.Create(
                user,
                request.Title,
                request.Content);   
            
            _noteRepository.Add(note);
            await _unitOfWork.Commit(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Content))
            {
                await HandleEmbeddings(note, cancellationToken);
            }
            var noteDto = _mapper.Map<NoteDto>(note);
            return Result.Success(new NotesContracts.CreateNoteResponse(noteDto));
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