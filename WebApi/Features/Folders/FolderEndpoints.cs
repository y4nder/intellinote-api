using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Features.Folders;

public class FolderEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var route = app.CreateApiGroup("folders", "Folders")
            .RequireAuthorization();

        route.MapGet("", async ([FromServices] ISender sender, [FromQuery]string? term = null, [FromQuery]int skip = 0, [FromQuery]int take = 10) =>
        {
            var query = new GetFolders.Query
            {
                Term = term,
                Skip = skip,
                Take = take
            };
            var response = await sender.Send(query);
            return response.ToHttpResult();
        }).AddProducedTypesWithoutValidation<FolderContracts.GetFoldersResponse>();
        
        route.MapGet("/{folderId:guid}", async (Guid folderId, ISender sender) =>
        {
            var response = await sender.Send(new GetFolder.Query{FolderId = folderId});
            return response.ToHttpResult();
        }).AddProducedTypesWithoutValidation<FolderContracts.GetFolderResponse>();
        
        route.MapPost("", async (ISender sender, FolderContracts.CreateFolderRequest request) =>
        {   
            var response = await sender.Send(request.Map());
            return response.ToHttpResult();
        }).AddProducedTypesWithoutValidation<FolderContracts.CreateFolderResponse>();
        
        route.MapPut("/{folderId:guid}", async (Guid folderId, ISender sender, [FromBody] FolderContracts.UpdateFolderRequest request) =>
        {
            var command = request.Map();
            command.FolderId = folderId;
            var response = await sender.Send(command);
            return response.ToHttpResult();
        }).AddProducedTypesWithoutValidation<FolderContracts.UpdateFolderResponse>();
        
        route.MapDelete("/{folderId:guid}", async (Guid folderId, ISender sender) =>
        {
            var response = await sender.Send(new DeleteFolder.Command{FolderId = folderId});
            return response.ToHttpResult();
        }).AddProducedTypesWithoutValidation<FolderContracts.DeleteFolderResponse>();

        route.MapPost("/{folderId:guid}/action", async (Guid folderId, ISender sender, [FromQuery] string type, [FromBody]FolderContracts.UpdateFolderNotesRequest request ) =>
        {
            if (!FolderContracts.AllowedActionTypes.Contains(type))
            {
                var errorMessage = "Invalid action type. Allowed types: " + string.Join(", ", FolderContracts.AllowedActionTypes) + ".";
                return Results.BadRequest(new { Error = errorMessage });   
            }
            
            if (type.Equals("add"))
            {
                var response = await sender.Send(new AddNotesToFolder
                {
                    FolderId = folderId,
                    NoteIds = request.NoteIds
                });
                return response.ToHttpResult();
            }
            else
            {
                var response = await sender.Send(new RemoveNotesToFolder
                {
                    FolderId = folderId,
                    NoteIds = request.NoteIds
                });
                return response.ToHttpResult();
            }
        });
    }
}