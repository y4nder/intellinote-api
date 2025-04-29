using Carter;
using WebApi.Extensions;

namespace WebApi.Features.Health;

public class HealthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var route = app.CreateApiGroup("health", "Health").AllowAnonymous();
        
        route.MapGet((""), () => "Healthy");
    }
}