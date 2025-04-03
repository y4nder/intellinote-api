using WebApi.Errors;

namespace WebApi.Extensions;

public static class EndpointExtensions
{
    private const string ApiPrefix = "api";

    public static RouteGroupBuilder CreateApiGroup(this IEndpointRouteBuilder route, string name, string? tag = null)
    {
        var apiGroup = route.MapGroup($"{ApiPrefix}/{name}");
        apiGroup.WithTags(tag ?? name);
        return apiGroup;
    }

    public static RouteHandlerBuilder AddProducedTypes<TResult>(this RouteHandlerBuilder route)
    {
        route.Produces<TResult>()
            .Produces<Error>(StatusCodes.Status400BadRequest)
            .Produces<ValidationError>(StatusCodes.Status422UnprocessableEntity);
        
        return route;
    }
    
    public static RouteHandlerBuilder AddProducedTypesWithoutValidation<TResult>(this RouteHandlerBuilder route)
    {
        route.Produces<TResult>()
            .Produces<Error>(StatusCodes.Status400BadRequest);
        
        return route;
    }

    public static RouteHandlerBuilder AddAdditionalProducedTypes(this RouteHandlerBuilder route,
        params BaseProducedType[] additionalProducedTypes)
    {
        foreach (var entry in additionalProducedTypes)
            route.Produces(entry.StatusCode, entry.Type);
        
        return route;
    }
}


public class ProducedType<T>(int statusCode) : BaseProducedType(typeof(T), statusCode);

public class BaseProducedType
{
    public Type Type { get; }
    public int StatusCode { get; }

    protected BaseProducedType(Type type, int statusCode)
    {
        Type = type;
        StatusCode = statusCode;
    }
}
