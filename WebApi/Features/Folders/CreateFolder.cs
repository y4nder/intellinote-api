using AutoMapper;
using FluentValidation;
using MediatR;
using Quartz;
using WebApi.Data.Entities;
using WebApi.Errors;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Features.Folders.Notifications;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Folders;

public class CreateFolder
{
    public class Command : IRequest<Result<FolderContracts.CreateFolderResponse>>
    {
        public String Name { get; set; } = null!;
        public String Description { get; set; } = String.Empty;
        public List<Guid> NoteIds { get; set; } = new List<Guid>();
        public bool Auto { get; set; } = false;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).MaximumLength(255);
            RuleFor(x => x.NoteIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("NoteIds must be unique.");
        }
    }
    
    internal sealed class Handler : IRequestHandler<Command, Result<FolderContracts.CreateFolderResponse>>
    {
        private readonly IValidator<Command> _validator;
        private readonly UserContext<User, string> _userContext;
        private readonly FolderRepository _folderRepository;
        private readonly NoteRepository _noteRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ISchedulerFactory _schedulerFactory; 
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public Handler(IValidator<Command> validator,
            UserContext<User, string> userContext,
            FolderRepository folderRepository,
            UnitOfWork unitOfWork,
            IMapper mapper,
            NoteRepository noteRepository,
            ISchedulerFactory schedulerFactory, IMediator mediator)
        {
            _validator = validator;
            _userContext = userContext;
            _folderRepository = folderRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _noteRepository = noteRepository;
            _schedulerFactory = schedulerFactory;
            _mediator = mediator;
        }

        public async Task<Result<FolderContracts.CreateFolderResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<FolderContracts.CreateFolderResponse>(
                    validationResult.Errors.ExtractToList());
            }
            
            var currentUser = await _userContext.GetCurrentUser();
            
            var folder = Folder.Create(request.Name, request.Description, currentUser);
            
            var notes = new List<Note>();
            
            if (request.NoteIds.Any())
            {
                notes = await _noteRepository.GetNotesByNoteIdsAsync(request.NoteIds);
                if (notes.Count != request.NoteIds.Count)
                    return Result.Failure<FolderContracts.CreateFolderResponse>(FolderErrors.SomeNotsNotFound);
                folder.AddNotes(notes);
                foreach (var note in notes)
                {
                    note.ForceUpdate();
                }
            }
            
            _folderRepository.Add(folder);
            
            var saved = await _unitOfWork.Commit(cancellationToken);
            
            if (saved.IsSuccess && notes.Any())
            {
                await _mediator.Publish(new DelegateFolderEmbeddingNotification
                {
                    FolderId = folder.Id,
                    Auto = request.Auto,
                    Notes = notes
                }, cancellationToken);
            }
            
            if(saved.IsFailure) return Result.Failure<FolderContracts.CreateFolderResponse>(saved.Error!);
            
            var folderDto = _mapper.Map<FolderWithDetailsDto>(folder);

            return Result.Success(new FolderContracts.CreateFolderResponse(folderDto, request.Auto));
        }
        
    }
}