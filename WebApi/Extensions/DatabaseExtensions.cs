using Microsoft.EntityFrameworkCore;
using WebApi.Errors.ErrorDefinitions;
using WebApi.ResultType;

namespace WebApi.Extensions;

/// <summary>
/// Provides extension methods for handling database operations in DbContext.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Saves changes to the database and dispatches domain events.  
    /// Returns a success or failure result based on the operation outcome.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Result object indicating success or failure.</returns>
    public static async Task<Result> SaveChangesOrFailAsync(
        this DbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Save changes to the database
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.ToUpper().Contains("UNIQUE") == true)
        {
            return Result.Failure(DatabaseErrors.UniqueConstraintViolation());
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure(DatabaseErrors.ConcurrencyConflict);
        }
        catch (DbUpdateException)
        {
            return Result.Failure(DatabaseErrors.DatabaseUpdateFailed);
        }
        catch (Exception)
        {
            return Result.Failure(DatabaseErrors.UnexpectedDatabaseError);
        }
    }
}