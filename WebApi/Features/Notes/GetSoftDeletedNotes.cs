using MediatR;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi.Features.Notes;

public class GetSoftDeletedNotes
{
    public record Query() : IRequest<NotesContracts.GetNotesResponse>;
    
    internal sealed class Handler : IRequestHandler<Query, NotesContracts.GetNotesResponse>
    {
        private readonly NoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;

        public Handler(NoteRepository noteRepository, UserContext<User, string> userContext)
        {
            _noteRepository = noteRepository;
            _userContext = userContext;
        }

        public async Task<NotesContracts.GetNotesResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            var notes = await _noteRepository.FindDeletedNotesWithProjection(_userContext.Id());

            return new NotesContracts.GetNotesResponse(notes, notes.Count);
        }
    }
}