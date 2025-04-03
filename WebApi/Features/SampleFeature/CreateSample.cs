using FluentValidation;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.SampleFeature;

public static class CreateSample
{
    public class Command : IRequest<Result<Guid>>
    {
        public string Name { get; set; } = null!;
        public string UniqueName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(4);
            RuleFor(x => x.UniqueName).NotEmpty().MinimumLength(4);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly SampleRepository _sampleRepository;
        private readonly IValidator<Command> _validator;
        private readonly UnitOfWork _unitOfWork;


        public Handler(SampleRepository sampleRepository, IValidator<Command> validator, UnitOfWork unitOfWork)
        {
            _sampleRepository = sampleRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(
                    validationResult.Errors
                    .ExtractToList()
                    .WithEndpoint(nameof(CreateSample)));
            }

            var sample = SampleEntity.Create(request.Name, request.UniqueName, request.Email);
            _sampleRepository.Add(sample);
            var result = await _unitOfWork.Commit(cancellationToken);
            if(result.IsFailure){
                return Result.Failure<Guid>(result.Error!);
            }
            
            return Result.Success(sample.Id);
        }
    }
}

