using MediatR;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.ResultType;

namespace WebApi.Features.SampleFeature;

public class GetAllSamples
{
    public record Query() : IRequest<Result<List<SampleEntity>>>;

    internal sealed class Handler : IRequestHandler<Query, Result<List<SampleEntity>>>
    {
        private readonly SampleRepository _repository;

        public Handler(SampleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<SampleEntity>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var samples = await _repository.GetAllAsync();
            return Result.Success(samples.ToList());
        }
    }
}