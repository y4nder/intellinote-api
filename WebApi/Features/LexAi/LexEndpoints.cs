using Carter;
using WebApi.Extensions;
using WebApi.Services.Agent.Agents;
using WebApi.Services.Agent.Dtos;

namespace WebApi.Features.LexAi;

public class LexEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var route = app.CreateApiGroup("lex", "Lex")
            .RequireAuthorization();

        route.MapPost("", async (PromptContracts.PromptNoteCreationDto request, ILexAgent lexAgent) =>
        {
            var response = await lexAgent.ProcessPromptAsync(request);
            return response;
        });
    }
}