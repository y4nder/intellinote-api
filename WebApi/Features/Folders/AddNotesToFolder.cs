using AutoMapper;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Features.Folders.Notifications;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.Repositories.Folder;
using WebApi.Repositories.Note;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Http;

namespace WebApi.Features.Folders;

public class AddNotesToFolder : IRequest<Result<AddNotesToFolderResponse>>
{
    public Guid FolderId { get; set; }
    public List<Guid> NoteIds { get; set; } = new List<Guid>();
}

public class AddNotesToFolderResponse
{
    public String Message { get; set; } = null!;
    public Guid FolderId { get; set; }
    public List<NoteDtoMinimal> Notes { get; set; } = new();
}

internal sealed class AddNotesToFolderHandler : IRequestHandler<AddNotesToFolder, Result<AddNotesToFolderResponse>>
{
    private readonly UserContext<User, string> _userContext;
    private readonly IFolderRepository _folderRepository;
    private readonly INoteRepository _noteRepository;
    private readonly UnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AddNotesToFolderHandler(
        UserContext<User, string> userContext,
        IFolderRepository folderRepository,
        INoteRepository noteRepository,
        UnitOfWork unitOfWork,
        IMediator mediator, 
        IMapper mapper)
    {
        _userContext = userContext;
        _folderRepository = folderRepository;
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _mapper = mapper;
    }
    // Don't waste time checking this function it's basically the same as RemoveNotesToFolder except for line 68
    public async Task<Result<AddNotesToFolderResponse>> Handle(AddNotesToFolder request, CancellationToken cancellationToken)
    {
        if (!request.NoteIds.Any()) 
            return Result.Failure<AddNotesToFolderResponse>(FolderErrors.NoIdsProvided);
        
        var folder = await _folderRepository.FindByIdAsyncWithNotes(request.FolderId);

        if (folder == null) 
            return Result.Failure<AddNotesToFolderResponse>(FolderErrors.FolderNotFound);
        
        
        var owned = OwnershipValidator.Ensure(folder, f => f.UserId == _userContext.Id());
        
        if (owned.IsFailure) return Result.Failure<AddNotesToFolderResponse>(owned.Error!);
        
        var notes = await _noteRepository.GetNotesByNoteIdsAsync(request.NoteIds);

        folder.AddNotes(notes); // this line here
        foreach (var note in notes)
        {
            note.ForceUpdate();
        }
        
        // commit changes
        await _unitOfWork.Commit(cancellationToken);
        
        // send for embedding
        await _mediator.Publish(new DelegateFolderEmbeddingNotification
        {
            FolderId = request.FolderId,
            Notes = folder.Notes,
            Auto = false,
        }, cancellationToken);
        
        var notesDto = _mapper.Map<List<NoteDtoMinimal>>(notes);
        
        return Result.Success(new AddNotesToFolderResponse
        {
            Message = $"{notes.Count} Notes were added to the folder",
            FolderId = request.FolderId,
            Notes = notesDto
        });
    }
}

    
