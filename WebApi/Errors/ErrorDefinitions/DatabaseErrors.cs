namespace WebApi.Errors.ErrorDefinitions;

public static class DatabaseErrors
{
    public static readonly Error DatabaseUpdateFailed = new(
        "Database.UpdateFailed",
        "A database update error occurred. Please try again later.",
        StatusCodes.Status500InternalServerError
    );

    public static Error UniqueConstraintViolation(string? constraintName = "") => new(
        "Database.UniqueConstraintViolation",
        !string.IsNullOrEmpty(constraintName) ? constraintName + " constraint was violated" 
            : "A record with this value already exists.",
        StatusCodes.Status409Conflict
    );
    
    

    public static readonly Error ConcurrencyConflict = new(
        "Database.ConcurrencyConflict",
        "The record was modified by another process. Please refresh and try again.",
        StatusCodes.Status409Conflict

    );

    public static readonly Error UnexpectedDatabaseError = new(
        "Database.UnexpectedError",
        "An unexpected database error occurred.",
        StatusCodes.Status500InternalServerError
    );
}