namespace WebApi.Errors.ErrorDefinitions;

public class NoteErrors
{
    public static readonly Error NoteNotFound = new Error(
        "NOT NOT FOUND",
        "The note was not found",
        StatusCodes.Status404NotFound);
}