namespace WebApi.Errors.ErrorDefinitions;

public class FolderErrors
{
    public static readonly Error SomeNotsNotFound = new Error(
        "SOME NOTES NOT FOUND",
        "some notes were not found",
        StatusCodes.Status400BadRequest);
}