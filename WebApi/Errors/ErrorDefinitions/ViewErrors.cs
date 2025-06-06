namespace WebApi.Errors.ErrorDefinitions;

public class ViewErrors
{
    public static readonly Error ViewNotFound = new Error(
        "VIEW NOT FOUND",
        "view was not found",
        StatusCodes.Status404NotFound);
}