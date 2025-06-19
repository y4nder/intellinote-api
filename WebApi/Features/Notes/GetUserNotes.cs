using MediatR;
using Pgvector;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.Repositories.Note;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Http;

namespace WebApi.Features.Notes;

public class GetUserNotes
{
    
    internal sealed class Handler : IRequestHandler<NotesContracts.GetNotesRequest, Result<NotesContracts.GetNotesResponse>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;
        private readonly EmbeddingService _embeddingService;

        public Handler(
            INoteRepository noteRepository, 
            UserContext<User, string> userContext, EmbeddingService embeddingService)
        {
            _noteRepository = noteRepository;
            _userContext = userContext;
            _embeddingService = embeddingService;
        }

        public async Task<Result<NotesContracts.GetNotesResponse>> Handle(NotesContracts.GetNotesRequest request, CancellationToken cancellationToken)
        {
            var currentUser = await _userContext.GetCurrentUser();
            
            // Vector? searchVector = null;
            // if (!string.IsNullOrEmpty(request.Term))
            // {
            //     searchVector = await _embeddingService.GenerateEmbeddings(request.Term);                
            // }
            
            // var result = await _noteRepository.GetAllNotesForUserAsync(currentUser.Id, searchVector, request.Skip, request.Take);
            var result = await _noteRepository.SearchNotesAsync(currentUser.Id, request.Term, request.Skip, request.Take);
            return Result.Success(new NotesContracts.GetNotesResponse(result.Items, result.TotalItems));
        }
    }
}