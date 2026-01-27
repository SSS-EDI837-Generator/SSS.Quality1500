namespace SSS.Quality1500.Domain.CQRS;

/// <summary>
/// Marker interface for queries (read operations).
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQuery<TResult>;

/// <summary>
/// Handler for processing queries.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles the query and returns the result.
    /// </summary>
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}
