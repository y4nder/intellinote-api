using MediatR;
using WebApi.Data.Entities;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Notes;

public class SummarizeNote
{
    public class Command : IRequest<Result<Response>>
    {
        public Guid NoteId { get; set; }
    }

    public class Response
    {
        public string Message { get; set; } = null!;
        public bool Approved { get; set; }
    }
    
    internal sealed class Handler : IRequestHandler<Command, Result<Response>>
    {
        private readonly NoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;

        public Handler(NoteRepository noteRepository, UserContext<User, string> userContext)
        {
            _noteRepository = noteRepository;
            _userContext = userContext;
        }

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.FindNoteWithProjection(request.NoteId);
            
            var owned = OwnershipValidator.Ensure(
                note,
                n => n.Author.Id == _userContext.Id()
            );
            if (owned.IsFailure) 
                return Result.Failure<Response>(owned.Error!);
            
            var text = $"{note!.Title} {note.Content}";

            return Result.Success(new Response
            {
                Message = "Approved",
                Approved = true
            });
        }
    }
}