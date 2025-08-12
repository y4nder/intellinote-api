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

public class RemoveNotesToFolder : IRequest<Result<RemoveNotesToFolderResponse>>
{
    public Guid FolderId { get; set; }
    public List<Guid> NoteIds { get; set; } = new List<Guid>();
}

public class RemoveNotesToFolderResponse
{
    public String Message { get; set; } = null!;
    public Guid FolderId { get; set; }
    public List<NoteDtoMinimal> Notes { get; set; } = new();
}

internal sealed class RemoveNotesToFolderHandler : IRequestHandler<RemoveNotesToFolder, Result<RemoveNotesToFolderResponse>>
{
    private readonly UserContext<User, string> _userContext;
    private readonly IFolderRepository _folderRepository;
    private readonly INoteRepository _noteRepository;
    private readonly UnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public RemoveNotesToFolderHandler(
        UserContext<User, string> userContext,
        IFolderRepository folderRepository,
        INoteRepository noteRepository,
        UnitOfWork unitOfWork,
        IMediator mediator, IMapper mapper)
    {
        _userContext = userContext;
        _folderRepository = folderRepository;
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _mapper = mapper;
    }
    // Don't waste time checking this function it's basically the same as AddNotesToFolder except for line 66
    public async Task<Result<RemoveNotesToFolderResponse>> Handle(RemoveNotesToFolder request, CancellationToken cancellationToken)
    {
        if (!request.NoteIds.Any()) 
            return Result.Failure<RemoveNotesToFolderResponse>(FolderErrors.NoIdsProvided);
        
        var folder = await _folderRepository.FindByIdAsyncWithNotes(request.FolderId);

        if (folder == null) 
            return Result.Failure<RemoveNotesToFolderResponse>(FolderErrors.FolderNotFound);
        
        var owned = OwnershipValidator.Ensure(folder, f => f.UserId == _userContext.Id());
        
        if (owned.IsFailure) return Result.Failure<RemoveNotesToFolderResponse>(owned.Error!);
        
        var notes = await _noteRepository.GetNotesByNoteIdsAsync(request.NoteIds);

        folder.RemoveNotes(notes); // this line here
        foreach (var note in notes)
        {
            note.ForceUpdate();
        }
        
        // commit changes
        await _unitOfWork.CommitAsync(cancellationToken);
        
        // send for embedding
        await _mediator.Publish(new DelegateFolderEmbeddingNotification
        {
            FolderId = request.FolderId,
            Notes = folder.Notes,
            Auto = false,
        }, cancellationToken);
        
        var notesDto = _mapper.Map<List<NoteDtoMinimal>>(notes);
        
        return Result.Success(new RemoveNotesToFolderResponse
        {
            Message = $"{notes.Count} Notes were removed to the folder",
            FolderId = request.FolderId,
            Notes = notesDto
        });
    }
}
