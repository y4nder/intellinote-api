using MediatR;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.Repositories.Note;
using WebApi.Services;
using WebApi.Services.Http;

namespace WebApi.Features.Notes;

public class GetSoftDeletedNotes
{
    public record Query() : IRequest<NotesContracts.GetNotesResponse>;
    
    internal sealed class Handler : IRequestHandler<Query, NotesContracts.GetNotesResponse>
    {
        private readonly INoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;

        public Handler(INoteRepository noteRepository, UserContext<User, string> userContext)
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