using MediatR;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Repositories;
using WebApi.ResultType;

namespace WebApi.Features.SampleFeature;

public class GetSampleEmail
{
    public class Query : IRequest<Result<Response>>
    {
        public string Email { get; set; } = String.Empty;
    }
    
    public class Response
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Response>>
    {
        private readonly SampleRepository _repository;

        public Handler(SampleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetSampleEntityByEmail(request.Email);

            if (result is null)
                return Result.Failure<Response>(SampleErrors.SampleNotFound);
            
            return Result.Success(new Response { Id = result.Id, Name = result.UniqueName });
        }
    }
}