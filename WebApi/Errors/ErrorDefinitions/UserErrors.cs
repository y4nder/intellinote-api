namespace WebApi.Errors.ErrorDefinitions;

public static class UserErrors
{
    public static readonly Error UserNotFoundOrNotLoggedIn = new Error(
        "UserNotFoundOrNotLoggedIn",
        "The user could not be found or logged in as input.",
        StatusCodes.Status404NotFound
    );

    public static readonly Error UserNotAuthenticated = new Error(
        "UserNotAuthenticated",
        "The user could not be authenticated.",
        StatusCodes.Status401Unauthorized
    );

    public static readonly Error UserNotFound = new Error(
        "UserNotFound",
        "The user could not be found.",
        StatusCodes.Status401Unauthorized
    );
}