using Carter;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Services.External;

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
                flaskApi = externalStatus
            };
        });
    }
}