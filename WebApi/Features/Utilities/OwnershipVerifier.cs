using WebApi.Errors.ErrorDefinitions;
using WebApi.ResultType;

namespace WebApi.Features.Utilities;

public static class OwnershipValidator
{
    public static Result Ensure<T>(
        T? entity, 
        Func<T, bool> isOwner)
        where T : class
    {
        if (entity is null)
        {
            return Result.Failure<T>(BaseErrors.ResourceNotFound(typeof(T)));
        }

        if (!isOwner(entity))
        {
            return Result.Failure<T>(BaseErrors.ForbiddenAccess(typeof(T)));
        }

        return Result.Success(entity);
    }
}
