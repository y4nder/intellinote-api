using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Features.Folders.Notifications;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

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
}

internal sealed class AddNotesToFolderHandler : IRequestHandler<AddNotesToFolder, Result<AddNotesToFolderResponse>>
{
    private readonly UserContext<User, string> _userContext;
    private readonly FolderRepository _folderRepository;
    private readonly NoteRepository _noteRepository;
    private readonly UnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public AddNotesToFolderHandler(
        UserContext<User, string> userContext,
        FolderRepository folderRepository,
        NoteRepository noteRepository,
        UnitOfWork unitOfWork,
        IMediator mediator)
    {
        _userContext = userContext;
        _folderRepository = folderRepository;
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }
    // Don't waste time checking this function it's basically the same as RemoveNotesToFolder except for line 63
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
        
        return Result.Success(new AddNotesToFolderResponse
        {
            Message = $"{notes.Count} Notes were added to the folder",
            FolderId = request.FolderId
        });
    }
}

    
