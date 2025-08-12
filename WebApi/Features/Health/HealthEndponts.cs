using Carter;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Services.External;
using WebApi.Services.Hubs;

// using WebApi.Data.Entities;
// using WebApi.Services.Hubs;
// using WebApi.Services.Parsers;

namespace WebApi.Features.Health;

public class HealthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var route = app.CreateApiGroup("health", "Health").AllowAnonymous();
        
        route.MapGet((""), async ([FromServices]GeneratedResponseService generatedResponseService) =>
        {
            var externalStatus = await generatedResponseService.HealthCheck();
            return new
            {
                dotnetApi="Healthy",
                aiApi = externalStatus
            };
        });
        
        // route.MapGet(("/note-schema"), () =>
        // {
        //     return "Note Schema";
        // }).AddProducedTypes<Note>();
        //
        // route.MapGet(("/note-minimal-dto"), () =>
        // {
        //     return "Note Schema";
        // }).AddProducedTypes<NoteDtoMinimal>();
        //
        // route.MapGet(("/note-dto"), () =>
        // {
        //     return "Note Schema";
        // }).AddProducedTypes<NoteDto>();
        //
        // route.MapPost("/check-block-schema", ([FromBody]string blocks, [FromServices] BlockNoteParserService blockNoteParserService) =>
        // {   
        //     blockNoteParserService.TryParse(blocks, out var result);
        //     var text = blockNoteParserService.Stringify(result);
        //     return text;
        // });
        //
        route.MapGet("/manual-socket", async ([FromServices] NoteHubService noteHubService) =>
        {
            await noteHubService.ManualDevNotify();
        });
    }
}