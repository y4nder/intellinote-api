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

        route.MapGet("", async (ISender sender) =>
        {
            var response = await sender.Send(new GetFolders.Query());
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
    }
}