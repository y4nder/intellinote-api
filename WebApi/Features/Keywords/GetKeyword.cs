using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Repositories;
using WebApi.ResultType;

namespace WebApi.Features.Keywords;

public class GetKeyword
{
    public class GetKeywordRequest : IRequest<Result<GetKeywordResponse>>
    {
        public Guid KeywordId { get; set; }
    }

    public class GetKeywordResponse
    {
        public KeywordDto Keyword { get; set; } = null!;
    }
    
    internal sealed class Handler : IRequestHandler<GetKeywordRequest, Result<GetKeywordResponse>>
    {
        private readonly KeywordRepository _repository;

        public Handler(KeywordRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<GetKeywordResponse>> Handle(GetKeywordRequest request, CancellationToken cancellationToken)
        {
            var keyword = await _repository.GetKeywordWithMappingAsync(request.KeywordId);
            return keyword == null ? 
                Result.Failure<GetKeywordResponse>(BaseErrors.ResourceNotFound(typeof(Keyword))) : 
                Result.Success(new GetKeywordResponse { Keyword = keyword });
        }
    }
}