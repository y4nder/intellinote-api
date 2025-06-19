using MediatR;
using WebApi.Data.Entities;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.Repositories.Note;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Http;

namespace WebApi.Features.Notes;

public class DeleteNote
{
    public class Command : IRequest<Result<NotesContracts.DeleteNoteResponse>>
    {
        public Guid NoteId { get; init; }
    }
    
    internal sealed class Handler : IRequestHandler
    <Command, Result<NotesContracts.DeleteNoteResponse>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserContext<User, string> _userContext;

        public Handler(INoteRepository noteRepository, UnitOfWork unitOfWork, UserContext<User, string> userContext)
        {
            _noteRepository = noteRepository;
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        public async Task<Result<NotesContracts.DeleteNoteResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingNote = await _noteRepository.FindByIdAsync(request.NoteId);
            var owned = OwnershipValidator.Ensure(existingNote, e => e.UserId == _userContext.Id());

            if (owned.IsFailure) return Result.Failure<NotesContracts.DeleteNoteResponse>(owned.Error!);
            
            _noteRepository.Delete(existingNote!);
            
             var deleted = await _unitOfWork.Commit(cancellationToken);
             
             if(deleted.IsFailure) return Result.Failure<NotesContracts.DeleteNoteResponse>(deleted.Error!);
             
             return Result.Success(new NotesContracts.DeleteNoteResponse(existingNote!.Id));
        }
    }
}