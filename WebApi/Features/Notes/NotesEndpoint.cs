using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Features.Notes.AutoAssignNote;

namespace WebApi.Features.Notes;

public class NotesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var route = app.CreateApiGroup("notes", "Notes")
            .RequireAuthorization();

        route.MapGet("", async ([FromServices]ISender sender, [FromQuery]string? term = null, [FromQuery]int skip = 0, [FromQuery]int take = 10) =>
        {
            var query = new NotesContracts.GetNotesRequest(term, skip, take);
            var response = await sender.Send(query);
            return response.ToHttpResult();
        }).AddProducedTypes<NotesContracts.GetNotesResponse>();
        
        route.MapGet("/{noteId:guid}", async (ISender sender, Guid noteId) =>
        {
            var query = new GetNote.Query { NoteId = noteId };
            var response = await sender.Send(query);
            return response.ToHttpResult();
        }).AddProducedTypesWithoutValidation<NotesContracts.GetNoteResponse>();
        

        route.MapPost("", async (ISender sender, NotesContracts.CreateNoteRequest request) =>
        {
            var response = await sender.Send(request.Map());
            return response.ToHttpResult();
        }).AddProducedTypes<NotesContracts.CreateNoteResponse>();

        route.MapPost("/ai/{noteId:guid}", async (ISender sender, Guid noteId) =>
        {
            var command = new SummarizeNote.Command{NoteId = noteId};
            var response = await sender.Send(command);
            return response.ToHttpResult();
        }).AddProducedTypesWithoutValidation<SummarizeNote.SummarizeNoteResponse>();

        route.MapPut("/{noteId:guid}", async (ISender sender, Guid noteId, [FromBody]NotesContracts.UpdateNoteRequest request) =>
        {
            var command = request.Map();
            command.NoteId = noteId;
            var response = await sender.Send(command);
            return response.ToHttpResult();
        }).AddProducedTypes<NotesContracts.UpdateNoteResponse>();

        route.MapDelete("/{noteId:guid}", async (ISender sender, Guid noteId) =>
        {
            var response = await sender.Send(new DeleteNote.Command{NoteId = noteId});
            response.ToHttpResult();
        }).AddProducedTypesWithoutValidation<NotesContracts.DeleteNoteResponse>();

        route.MapPost("/{noteId:guid}/assign", async (ISender sender, Guid noteId) =>
        {
            var response = await sender.Send(new GetPotentialFolders.AssignNoteFolderRequest
            {
                NoteId = noteId
            });
            return response.ToHttpResult();
        }).AddProducedTypesWithoutValidation<GetPotentialFolders.AssignNoteFolderResponse>();
    }
}