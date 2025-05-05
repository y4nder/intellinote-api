namespace WebApi.Errors.ErrorDefinitions;

public class FolderErrors
{
    public static readonly Error FolderNotFound = new Error(
        "FOLDER NOT FOUND",
        "folder was not found",
        StatusCodes.Status404NotFound);

    public static readonly Error SomeNotsNotFound = new Error(
        "SOME NOTES NOT FOUND",
        "some notes were not found",
        StatusCodes.Status400BadRequest);
    
    public static readonly Error NoIdsProvided = new Error(
        "NO NOTE IDS PROVIDED",
        "no note ids provided",
        StatusCodes.Status400BadRequest);
}