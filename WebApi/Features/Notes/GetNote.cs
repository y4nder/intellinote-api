using MediatR;
using WebApi.Data.Entities;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Notes;

public class GetNote
{
    public class Query : IRequest<Result<NotesContracts.GetNoteResponse>>
    {
        public Guid NoteId { get; set; }
    };
    
    internal sealed class Handler : IRequestHandler<Query, Result<NotesContracts.GetNoteResponse>>
    {
        private readonly NoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;
        public Handler(NoteRepository noteRepository, UserContext<User, string> userContext)
        {
            _noteRepository = noteRepository;
            _userContext = userContext;
        }

        public async Task<Result<NotesContracts.GetNoteResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.FindNoteWithProjection(request.NoteId);

            var owned = OwnershipValidator.Ensure(
                note,
                n => n.Author.Id == _userContext.Id()
            );

            if (owned.IsFailure) 
                return Result.Failure<NotesContracts.GetNoteResponse>(owned.Error!);
            
            return Result.Success(new NotesContracts.GetNoteResponse(note!));
        }
    }
}