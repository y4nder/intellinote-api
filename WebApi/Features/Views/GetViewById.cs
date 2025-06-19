using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Repositories;
using WebApi.Repositories.View;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Http;

namespace WebApi.Features.Views;

public class GetViewById
{
    public class GetViewByIdRequest : IRequest<Result<ViewResponseDto>>
    {
        public Guid ViewId { get; set; }
    }
    
    internal sealed class Handler : IRequestHandler<GetViewByIdRequest, Result<ViewResponseDto>>
    {
        private readonly IViewRepository _viewRepository;
        private readonly UserContext<User, string> _userContext;

        public Handler(IViewRepository viewRepository, UserContext<User, string> userContext)
        {
            _viewRepository = viewRepository;
            _userContext = userContext;
        }

        public async Task<Result<ViewResponseDto>> Handle(GetViewByIdRequest request, CancellationToken cancellationToken)
        {
            var view = await _viewRepository.GetViewDtoByIdAsync(request.ViewId, _userContext.Id());
            return view ?? Result.Failure<ViewResponseDto>(ViewErrors.ViewNotFound);
        }
    }
}