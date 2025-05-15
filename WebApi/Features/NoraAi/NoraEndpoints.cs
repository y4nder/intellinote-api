using Carter;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Services.Agent.Agents;
using WebApi.Services.Agent.Dtos;

namespace WebApi.Features.NoraAi;

public class NoraEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // done add authorization
        var route = app.CreateApiGroup("nora", "Nora")
            .RequireAuthorization();

        route.MapPost("/chat", async (PromptContracts.PromptRequestDto request, 
            [FromServices] INoraAgent noraAgent) =>
        {
            var response = await noraAgent.ProcessPromptAsync(request);
            return response;
        }).Produces<PromptContracts.PromptResponseDto>();
    }
}