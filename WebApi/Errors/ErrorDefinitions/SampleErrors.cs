namespace WebApi.Errors.ErrorDefinitions;

public static class SampleErrors
{
    public static readonly Error SampleNotFound = new Error(
            "SAMPLE NOT FOUND",
            "The sample entity was not found",
            StatusCodes.Status404NotFound);
}