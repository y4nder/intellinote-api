// using Carter;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Errors;
using WebApi.Extensions;

namespace WebApi.Features.SampleFeature;

public class SampleEndpoints 
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var sampleRoute = app.CreateApiGroup("sample", "Sample API");
        
        sampleRoute.MapGet("{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetSample.Query { Id = id };
            var response = await sender.Send(query);
            return response.ToHttpResult();
        })
        .AddProducedTypes<SampleResponse>()
        .AddAdditionalProducedTypes(
            new ProducedType<Error>(StatusCodes.Status404NotFound),
            new ProducedType<ValidationError>(StatusCodes.Status409Conflict)
        );

        sampleRoute.MapPost("", async (CreateSampleRequest request, ISender sender) =>
        {
            var response = await sender.Send(request.Map());
            return response.ToHttpResult();
        })
        .AddProducedTypes<Guid>()
        .AddAdditionalProducedTypes(
            new ProducedType<Error>(StatusCodes.Status409Conflict)    
        );

        sampleRoute.MapGet("", async ([FromQuery]String uniqueName, ISender sender) =>
        {
            var request = new GetUniqueSample.Query { UniqueName = uniqueName };
            var response = await sender.Send(request);
            return response.ToHttpResult();
        }).AddProducedTypes<GetUniqueSample.Response>();

        sampleRoute.MapPatch("", async (UpdateSampleContract request, ISender sender) =>
        {
            var response = await sender.Send(request.Map());
            return response.ToHttpResult();
        });

        sampleRoute.MapGet("/all", async (ISender sender) =>
        {
            var response = await sender.Send(new GetAllSamples.Query());
            return response.ToHttpResult();
        });

        sampleRoute.MapGet("/email", async ([FromQuery]string email, ISender sender) =>
        {
            var request = new GetSampleEmail.Query { Email = email };
            var response = await sender.Send(request);
            return response.ToHttpResult();
        }).AddProducedTypes<GetUniqueSample.Response>();
    }
}