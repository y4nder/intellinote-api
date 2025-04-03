using WebApi.Errors;
using WebApi.ResultType;

namespace WebApi.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult<TValue>(this Result<TValue> response)
    {
        return response.Match(
            Results.Ok,
            ExtractErrorResult
        );
    }
    
    private static readonly Dictionary<int, Func<IError, IResult>> ErrorHandlers = new()
    {
        { StatusCodes.Status400BadRequest, Results.BadRequest },
        { StatusCodes.Status401Unauthorized, _ => Results.Unauthorized() },
        { StatusCodes.Status403Forbidden, _ => Results.Forbid() },
        { StatusCodes.Status404NotFound, Results.NotFound },
        { StatusCodes.Status409Conflict, Results.Conflict },
        { StatusCodes.Status422UnprocessableEntity, Results.UnprocessableEntity }
    };

    private static IResult ExtractErrorResult(IError error)
    {
        return error switch
        {
            Error { StatusCode: not null } e when ErrorHandlers.TryGetValue(e.StatusCode.Value, out var handler) =>
                handler(e),
            Error e => Results.Problem("An error occurred", statusCode: StatusCodes.Status500InternalServerError),
            ValidationError v => Results.UnprocessableEntity(v),
            _ => Results.Problem("An unexpected error occurred", statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    public static IResult AsHttpResult(this IError error)
    {
        return ExtractErrorResult(error);
    }

    public static Exception AsThrowableException(this IError error)
    {
        return new ErrorException(error);
    }
}