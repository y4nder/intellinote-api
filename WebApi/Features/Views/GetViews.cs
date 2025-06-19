using MediatR;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.Repositories.View;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Http;

namespace WebApi.Features.Views;

public class GetViews
{
    public class GetViewsQuery : IRequest<Result<GetViewsResponse>>;
    
    public class GetViewsResponse
    {
        public List<ViewResponseDto> Views { get; set; } = new();
    }

    internal sealed class Handler : IRequestHandler<GetViewsQuery, Result<GetViewsResponse>>
    {
        private readonly IViewRepository _repository;
        private readonly UserContext<User, string> _userContext;
 
        public Handler(IViewRepository repository, UserContext<User, string> userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<Result<GetViewsResponse>> Handle(GetViewsQuery request, CancellationToken cancellationToken)
        {
            var views = await _repository.GetViewsByUserId(_userContext.Id());
            return new GetViewsResponse
            {
                Views = views
            };
        }
    }
}