namespace WebApi.Errors.ErrorDefinitions;

public static class KeywordErrors
{
    public static readonly Error BatchKeywordFailed = new Error(
        "BatchKeywordFailed",
        "Failed to process batch keywords.",
        StatusCodes.Status400BadRequest
    );
    
    public static readonly Error KeywordAlreadyExists = new Error(
            "KeywordAlreadyExists",
            "A keyword already exists in resource.",
            StatusCodes.Status400BadRequest
    );
    
    public static readonly Error InvalidKeywordIds = new Error(
        "InvalidKeywordIds",
        "Specified keyword id/s are invalid.",
        StatusCodes.Status400BadRequest
    );
}
