using AutoMapper;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Folders;

public class UpdateFolder
{
    public class Command : IRequest<Result<FolderContracts.UpdateFolderResponse>>
    {
        public Guid FolderId { get; set; }
        public String? Name { get; set; }
        public String? Description { get; set; }
    }
    
    internal sealed class Handler : IRequestHandler<Command, Result<FolderContracts.UpdateFolderResponse>>
    {
        private readonly FolderRepository _repository;
        private readonly UserContext<User,string> _userContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public Handler(FolderRepository repository, UserContext<User, string> userContext, UnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _userContext = userContext;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FolderContracts.UpdateFolderResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _repository.FindByIdAsync(request.FolderId);

            var owned = OwnershipValidator.Ensure(
                folder,
                f => f.UserId == _userContext.Id()
            );
            
            if(owned.IsFailure)
                return Result.Failure<FolderContracts.UpdateFolderResponse>(owned.Error!);
            
            folder!.Name = request.Name ?? folder.Name;
            folder.Description = request.Description ?? folder.Description;

            var saved = await _unitOfWork.Commit(cancellationToken);
            
            if(saved.IsFailure)
                return Result.Failure<FolderContracts.UpdateFolderResponse>(saved.Error!);

            return Result.Success(
                new FolderContracts.UpdateFolderResponse(
                    _mapper.Map<FolderWithDetailsDto>(folder)
                )
            );
        }
    }
}