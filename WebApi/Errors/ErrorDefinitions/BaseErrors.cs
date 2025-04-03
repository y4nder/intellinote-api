namespace WebApi.Errors.ErrorDefinitions;

public class BaseErrors
{
    public static Error ResourceNotFound(Type resourceType)
    {
        return new Error(
            "Resource Not Found",
            $"{resourceType.Name} was not found",
            StatusCodes.Status404NotFound
        );
    }

    public static Error ForbiddenAccess(Type resourceType)
    {
        return new Error(
            "Resource Forbidden",
            $"{resourceType.Name} is forbidden",
            StatusCodes.Status403Forbidden
        );
    }
}