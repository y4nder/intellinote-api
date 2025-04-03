using AutoMapper;
using FluentValidation;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Folders;

public class CreateFolder
{
    public class Command : IRequest<Result<FolderContracts.CreateFolderResponse>>
    {
        public String Name { get; set; } = null!;
        public String Description { get; set; } = String.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).MaximumLength(255);
        }
    }
    
    internal sealed class Handler : IRequestHandler<Command, Result<FolderContracts.CreateFolderResponse>>
    {
        private readonly IValidator<Command> _validator;
        private readonly UserContext<User, string> _userContext;
        private readonly FolderRepository _folderRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public Handler(IValidator<Command> validator, UserContext<User, string> userContext, FolderRepository folderRepository, UnitOfWork unitOfWork, IMapper mapper)
        {
            _validator = validator;
            _userContext = userContext;
            _folderRepository = folderRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FolderContracts.CreateFolderResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<FolderContracts.CreateFolderResponse>(
                    validationResult.Errors.ExtractToList());
            }
            
            var currentUser = await _userContext.GetCurrentUser();
            
            var folder = Folder.Create(request.Name, request.Description, currentUser);
            
            _folderRepository.Add(folder);
            
            var saved = await _unitOfWork.Commit(cancellationToken);
            
            if(saved.IsFailure) return Result.Failure<FolderContracts.CreateFolderResponse>(saved.Error!);
            
            var folderDto = _mapper.Map<FolderWithDetailsDto>(folder);

            return Result.Success(new FolderContracts.CreateFolderResponse(folderDto));
        }
    }
}