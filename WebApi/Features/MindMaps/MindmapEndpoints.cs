using Carter;
using MediatR;
using WebApi.Extensions;

namespace WebApi.Features.MindMaps;

public class MindmapEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var route = app.CreateApiGroup("mindmaps", "Mindmaps").RequireAuthorization();

        route.MapPost("", async (CreateMindMap.CreateMindMapRequest request, ISender sender) =>
        {
            var response = await sender.Send(request);
            return response;
        });
    }
}