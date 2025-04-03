namespace WebApi.Errors.ErrorDefinitions;

public static class KeywordErrors
{
    public static readonly Error BatchKeywordFailed = new Error(
        "BatchKeywordFailed",
        "Failed to process batch keywords.",
        StatusCodes.Status400BadRequest
    );
}