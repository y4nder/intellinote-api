using MediatR;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Notes;

public class GetUserNotes
{
    
    internal sealed class Handler : IRequestHandler<NotesContracts.GetNotesRequest, Result<NotesContracts.GetNotesResponse>>
    {
        private readonly NoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;

        public Handler(
            NoteRepository noteRepository, 
            UserContext<User, string> userContext)
        {
            _noteRepository = noteRepository;
            _userContext = userContext;
        }

        public async Task<Result<NotesContracts.GetNotesResponse>> Handle(NotesContracts.GetNotesRequest request, CancellationToken cancellationToken)
        {
            var currentUser = await _userContext.GetCurrentUser();
            var notes = await _noteRepository.GetAllNotesForUserAsync(currentUser.Id);
            return Result.Success(new NotesContracts.GetNotesResponse(notes));
        }
    }
}