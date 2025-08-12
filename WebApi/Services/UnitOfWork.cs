using WebApi.Data;
using WebApi.Extensions;
using WebApi.ResultType;

namespace WebApi.Services;

/// <summary>
/// Implements the Unit of Work pattern to manage database transactions and domain event dispatching.
/// Ensures atomicity and consistency when saving changes.
/// </summary>
public class UnitOfWork
{
    private readonly ApplicationDbContext _context;
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The application's database context.</param>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Commits all tracked entity changes to the database and dispatches domain events.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> indicating success or failure. 
    /// Returns <c>true</c> if the commit is successful; otherwise, returns an error.
    /// </returns>
    public async Task<Result> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesOrFailAsync(cancellationToken);
    }
}