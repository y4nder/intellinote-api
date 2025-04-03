using MediatR;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.ResultType;

namespace WebApi.Features.Keywords;

public class GetKeywords
{
    public class GetKeywordsRequest() : IRequest<Result<GetKeywordsResponse>> 
    { }

    public class GetKeywordsResponse()
    {
        public List<KeywordDto> Keywords { get; set; } = new List<KeywordDto>();
    }
    
    internal sealed class Handler : IRequestHandler<GetKeywordsRequest, Result<GetKeywordsResponse>>
    {
        private readonly KeywordRepository _repository;

        public Handler(KeywordRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<GetKeywordsResponse>> Handle(GetKeywordsRequest request, CancellationToken cancellationToken)
        {
            var keywords = await _repository.GetAllKeywordsWithMappingAsync();
            
            return new GetKeywordsResponse(){Keywords = keywords};
        }
    }
}