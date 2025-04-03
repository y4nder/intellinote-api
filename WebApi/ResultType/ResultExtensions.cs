using WebApi.Errors;

namespace WebApi.ResultType;

public static class ResultExtensions
{
    // Match for Result (non-generic)
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<IError, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error!);
    }

    // Match for Result<TValue> (generic)
    public static TOut Match<TValue, TOut>(
        this Result<TValue> result,
        Func<TValue, TOut> onSuccess,
        Func<IError, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error!);
    }
}