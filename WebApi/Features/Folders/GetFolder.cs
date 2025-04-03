using MediatR;
using WebApi.Data.Entities;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Folders;

public class GetFolder
{
    public class Query : IRequest<Result<FolderContracts.GetFolderResponse>>
    {
        public Guid FolderId { get; set; }
    };
    
    internal sealed class Handler : IRequestHandler<Query, Result<FolderContracts.GetFolderResponse>>
    {
        private readonly FolderRepository _repository;
        private readonly UserContext<User, string> _userContext;

        public Handler(FolderRepository repository, UserContext<User, string> userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<Result<FolderContracts.GetFolderResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var folder = await _repository.GetFolderWithDetailsAsync(request.FolderId);

            var owned = OwnershipValidator.Ensure(
                folder, 
                f => f.Author.Id == _userContext.Id());
            
            if(owned.IsFailure) 
                return Result.Failure<FolderContracts.GetFolderResponse>(owned.Error!);
            
            
            return Result.Success(new FolderContracts.GetFolderResponse(folder!));   
        }
    }
}