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
        // todo add authorization
        var route = app.CreateApiGroup("nora", "Nora")
            .AllowAnonymous();

        route.MapPost("/chat", async (PromptContracts.PromptRequestDto request, 
            [FromServices] IChatAgent<PromptContracts.PromptRequestDto, PromptContracts.PromptResponseDto> chatAgent) =>
        {
            var response = await chatAgent.ProcessPromptAsync(request);
            return response;
        });
    }
}