using FluentValidation;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Views;

public class CreateView
{
    public class CreateViewRequest : IRequest<Result<ViewResponseDto>>
    {
        public string Name { get; set; } = null!;
        public string FilterObject { get; set; } = null!;
    }

    internal class Validator : AbstractValidator<CreateViewRequest>
    {
        public Validator()
        {
            RuleFor(v => v.Name).NotEmpty().WithMessage("Name cannot be empty");
            RuleFor(v => v.FilterObject).NotEmpty().WithMessage("FilterObject cannot be empty");
        }
    }

    internal sealed class Handler : IRequestHandler<CreateViewRequest, Result<ViewResponseDto>>
    {
        private readonly ViewRepository _viewRepository;
        private readonly UserContext<User, string> _userContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly AbstractValidator<CreateViewRequest> _validator;

        public Handler(ViewRepository viewRepository, UserContext<User, string> userContext, UnitOfWork unitOfWork, AbstractValidator<CreateViewRequest> validator)
        {
            _viewRepository = viewRepository;
            _userContext = userContext;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<ViewResponseDto>> Handle(CreateViewRequest request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) Result.Failure<ViewResponseDto>(validation.Errors.ExtractToList());

            var user = await _userContext.GetCurrentUser();
            var newView = View.Create(user, request.Name, request.FilterObject);
            _viewRepository.Add(newView);
            var saved = await _unitOfWork.Commit(cancellationToken);
            if(saved.IsFailure) return Result.Failure<ViewResponseDto>(saved.Error!);

            return new ViewResponseDto
            {
                Id = newView.Id,
                Name = newView.Name,
                FilterCondition = newView.FilterCondition
            };
        }
    }
}