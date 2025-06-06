using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Entities;
using WebApi.Extensions;
using WebApi.ResultType;

namespace WebApi.Features.Views;

public class ViewEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var route = app.CreateApiGroup("views", "Views").RequireAuthorization();
        
        route.MapPost("", async ([FromBody] CreateView.CreateViewRequest request, ISender sender) =>
        {
            var response = await sender.Send(request);
            return response.ToHttpResult();
        }).Produces<ViewResponseDto>();
        
        route.MapGet("", async (ISender sender) =>
        {
            var response = await sender.Send(new GetViews.GetViewsQuery());
            return response.ToHttpResult();
        }).Produces<GetViews.GetViewsResponse>();
        
        route.MapGet("/{viewId:guid}", async ([FromQuery] Guid viewId, ISender sender) =>
        {
            var response = await sender.Send(new GetViewById.GetViewByIdRequest { ViewId = viewId });
            return response.ToHttpResult();
        });
        
        route.MapPut("/{viewId:guid}", async ( [FromQuery] Guid viewId, [FromBody] UpdateVIew.UpdateViewRequest request, ISender sender) =>
        {
            var response = await sender.Send(request);
            return response.ToHttpResult();
        });

        route.MapPost("/auto", async (AutoCreateView.Request request, ISender sender) =>
        {
            var response = await sender.Send(request);
            return response;
        });

    }
}