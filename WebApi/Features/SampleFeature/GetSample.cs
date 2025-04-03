using MediatR;
using WebApi.Contracts;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Repositories;
using WebApi.ResultType;

namespace WebApi.Features.SampleFeature;

public static class GetSample
{
    public class Query : IRequest<Result<SampleResponse>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<SampleResponse>>
    {
        private readonly SampleRepository _repository;

        public Handler(SampleRepository repository)
        {
            _repository = repository;
        }
    
        
        public async Task<Result<SampleResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var sample = await _repository.FindByIdAsync(request.Id);
            if (sample == null) return Result.Failure<SampleResponse>(SampleErrors.SampleNotFound);
            var response = new SampleResponse
            {
                Id = sample.Id,
                Name = sample.Name
            };
            return Result.Success(response);
        }
    }
}