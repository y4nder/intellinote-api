using Carter;
using MediatR;
using WebApi.Extensions;

namespace WebApi.Features.Keywords;

public class KeywordEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var route = app.CreateApiGroup("keywords")
            .RequireAuthorization();

        route.MapGet("", async (ISender sender) =>
        {
            var response = await sender.Send(new GetKeywords.GetKeywordsRequest());
            return response.ToHttpResult();
        }).AddProducedTypesWithoutValidation<GetKeywords.GetKeywordsResponse>();

        route.MapPost("/note/{noteId:guid}", async (Guid noteId, ISender sender, AddNoteKeywords.AddNoteKeywordRequest request ) =>
        {
            var command = new AddNoteKeywords.AddNoteKeywordsCommand
            {
                NoteId = noteId,
                Keywords = request.Keywords
            };
            var response = await sender.Send(command);
            return response.ToHttpResult();
        }).AddProducedTypes<AddNoteKeywords.AddNoteKeywordsResponse>();
        
        // route.MapPost("/batch", async (ISender sender, CreateKeywordsBatch.CreateKeywordsBatchRequest request) =>
        // {   
        //     var response = await sender.Send(request);
        //     return response.ToHttpResult();
        // }).AddProducedTypes<CreateKeywordsBatch.CreateKeywordsBatchResponse>();

    }
}