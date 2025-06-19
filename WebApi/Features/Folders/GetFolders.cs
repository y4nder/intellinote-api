using MediatR;
using Pgvector;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.Repositories.Folder;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Http;

namespace WebApi.Features.Folders;

public class GetFolders
{
    public class Query : IRequest<Result<FolderContracts.GetFoldersResponse>>
    {
        public string? Term { get; set; } = null!;
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
    
    internal sealed class Handler: IRequestHandler<Query, Result<FolderContracts.GetFoldersResponse>>
    {
        private readonly IFolderRepository _repository;
        private readonly UserContext<User, string> _userContext;
        private readonly EmbeddingService _embeddingService;

        public Handler(
            IFolderRepository repository,
            UserContext<User, string> userContext,
            EmbeddingService embeddingService)
        {
            _repository = repository;
            _userContext = userContext;
            _embeddingService = embeddingService;
        }

        public async Task<Result<FolderContracts.GetFoldersResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUser = await _userContext.GetCurrentUser();
            
            Vector? searchVector = null;
            if (!string.IsNullOrEmpty(request.Term))
            {
                searchVector = await _embeddingService.GenerateEmbeddings(request.Term);                
            }
            
            var folders = await _repository.GetFoldersWithDetailsAsync(currentUser.Id, searchVector, request.Skip, request.Take);

            return Result.Success(new FolderContracts.GetFoldersResponse(
                folders.Items, folders.TotalItems
            ));
        }
    }
}