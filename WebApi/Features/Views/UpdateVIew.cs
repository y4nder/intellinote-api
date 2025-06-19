using FluentValidation;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Repositories;
using WebApi.Repositories.View;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Http;

namespace WebApi.Features.Views;

public class UpdateVIew
{
    public class UpdateViewRequest : IRequest<Result<ViewResponseDto>>
    {
        public Guid ViewId { get; set; }
        public string? Name { get; set; }
        public string? FilterObject { get; set; }
    }

    public class Validator : AbstractValidator<UpdateViewRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ViewId).NotEmpty();
            
            When(x => x.Name is null, () =>
            {
                RuleFor(x => x.FilterObject)
                    .NotEmpty()
                    .WithMessage("Name should not be empty if Name is null.");
            });
            
            When(x => x.FilterObject is null, () => {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("Name should not be empty if FilterObject is null.");
            });

        }
    }
    
    internal sealed class Handler : IRequestHandler<UpdateViewRequest, Result<ViewResponseDto>>
    {
        private readonly IViewRepository _viewRepository;
        private readonly UserContext<User, string> _userContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly IValidator<UpdateViewRequest> _validator;


        public Handler(IViewRepository viewRepository, UserContext<User, string> userContext, UnitOfWork unitOfWork, IValidator<UpdateViewRequest> validator)
        {
            _viewRepository = viewRepository;
            _userContext = userContext;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<ViewResponseDto>> Handle(UpdateViewRequest request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            
            if(!validation.IsValid) 
                return Result.Failure<ViewResponseDto>(validation.Errors.ExtractToList());
            
            var view = await _viewRepository.GetViewByIdAsync(request.ViewId, _userContext.Id());
            
            if(view == null) 
                return Result.Failure<ViewResponseDto>(ViewErrors.ViewNotFound);
            
            view.Update(request.Name, request.FilterObject);
            
            var updated = await _unitOfWork.Commit(cancellationToken);
            if(updated.IsFailure)return Result.Failure<ViewResponseDto>(updated.Error!);

            return new ViewResponseDto
            {
                Id = view.Id,
                Name = view.Name,
                FilterCondition = view.FilterCondition
            };
        }
    }
}