using AutoMapper;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Folders;

public class DeleteFolder
{
    public class Command : IRequest<Result<FolderContracts.DeleteFolderResponse>>
    {
        public Guid FolderId { get; set; }
    }
    
    internal sealed class Handler : IRequestHandler<Command, Result<FolderContracts.DeleteFolderResponse>>
    {
        private readonly FolderRepository _repository;
        private readonly UserContext<User, string> _userContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public Handler(FolderRepository repository, UserContext<User, string> userContext, UnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _userContext = userContext;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FolderContracts.DeleteFolderResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _repository.FindByIdAsync(request.FolderId);

            var owned = OwnershipValidator.Ensure(folder, f => f.UserId == _userContext.Id());
            
            if(owned.IsFailure) return Result.Failure<FolderContracts.DeleteFolderResponse>(owned.Error!);
            
            _repository.Delete(folder!);
            
            var deleted = await _unitOfWork.Commit(cancellationToken);
            
            if(deleted.IsFailure) return Result.Failure<FolderContracts.DeleteFolderResponse>(deleted.Error!);

            return Result.Success(new FolderContracts.DeleteFolderResponse(folder!.Id));
        }
    }
}