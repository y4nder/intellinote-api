using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.SampleFeature;

public class UpdateSample
{
    public class Command : IRequest<Result<Response>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;    
        public string UniqueName { get; set; } = null!;
    }
    public class Response
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = String.Empty;
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Response>>
    {
        private readonly SampleRepository _repository;
        private readonly UnitOfWork _unitOfWork;

        public Handler(SampleRepository repository, UnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingSampleEntity = await _repository.FindByIdAsync(request.Id);
            if(existingSampleEntity == null) return Result.Failure<Response>(SampleErrors.SampleNotFound);
            
            existingSampleEntity.Update(new SampleEntity { Name = request.Name, UniqueName = request.UniqueName });
            
            var updated = await _unitOfWork.Commit(cancellationToken);  
            
            if(updated.IsFailure) return Result.Failure<Response>(updated.Error!);

            return Result.Success(new Response
            {
                Id = existingSampleEntity.Id,
                Message = "Updated"
            });
        }
    }
}

