using AutoMapper;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Entities;
using WebApi.Extensions;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi.Features.Views;

public class ViewEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var route = app.CreateApiGroup("views", "Views").RequireAuthorization();

        route.MapPost("", async ([FromBody] ViewDto request,
            [FromServices] ViewRepository repository,
            [FromServices] UserContext<User, string> userContext,
            [FromServices] UnitOfWork unitOfWork) =>
        {
            var view = View.Create(userContext.GetCurrentUser().Result, request.Name, request.FilterObject);
            repository.Add(view);
            await unitOfWork.Commit(CancellationToken.None);

            return new ViewResponseDto
            {
                Id = view.Id,
                Name = view.Name,
                FilterCondition = view.FilterCondition
            };
        });

        route.MapGet("/{viewId:guid}", async (
            Guid viewId,
            [FromServices] ViewRepository viewRepository
        ) =>
        {
            var view = await viewRepository.FindByIdAsync(viewId);
            if(view is null) throw new KeyNotFoundException("View not found.");
            return new ViewResponseDto
            {
                Id = view.Id,
                Name = view.Name,
                FilterCondition = view.FilterCondition
            };
        });
        
        route.MapGet("", async (
            [FromServices] ViewRepository viewRepository,
            [FromServices] UserContext<User, string> userContext) =>
        {
            var views = await viewRepository.GetViewsByUserId(userContext.Id());
            return new GetViewsResponse
            {
                Views = views
            };
        }).Produces<GetViewsResponse>();

        route.MapPut("/{viewId:guid}", async (
            Guid viewId,
            [FromBody] ViewDto request,
            [FromServices] ViewRepository viewRepository,
            [FromServices] UserContext<User, string> userContext,
            [FromServices] UnitOfWork unitOfWork) =>
        {
            var view = await viewRepository.FindByIdAsync(viewId);
            
            if(view is null) throw new KeyNotFoundException("View not found.");

            view.Name = request.Name;
            view.FilterCondition = request.FilterObject;
            await unitOfWork.Commit(CancellationToken.None);
            
            return new ViewResponseDto
            {
                Id = view.Id,   
                Name = view.Name,
                FilterCondition = view.FilterCondition
            };
        });

        route.MapPost("/auto", async (AutoCreateView.Request request, ISender sender) =>
        {
            var response = await sender.Send(request);
            return response;
        });

    }

    public class GetViewsResponse
    {
        public List<ViewResponseDto> Views { get; set; } = new();
    }

    public class ViewDto
    {
        public string Name { get; set; } = null!;
        public string FilterObject { get; set; } = null!;
    }

    public class CreateViewResponse
    {
        public Guid Id { get; set; }
    }
}