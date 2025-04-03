using MediatR;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Folders;

public class GetFolders
{
    public class Query : IRequest<Result<FolderContracts.GetFoldersResponse>>;
    
    internal sealed class Handler: IRequestHandler<Query, Result<FolderContracts.GetFoldersResponse>>
    {
        private readonly FolderRepository _repository;
        private readonly UserContext<User, string> _userContext;

        public Handler(FolderRepository repository, UserContext<User, string> userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<Result<FolderContracts.GetFoldersResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUser = await _userContext.GetCurrentUser();
            
            var folders = await _repository.GetFoldersWithDetailsAsync(currentUser.Id);

            return Result.Success(new FolderContracts.GetFoldersResponse(folders));
        }
    }
}