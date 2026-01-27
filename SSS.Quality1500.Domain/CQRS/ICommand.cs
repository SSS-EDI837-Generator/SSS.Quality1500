namespace SSS.Quality1500.Domain.CQRS;

/// <summary>
/// Marker interface for commands (write operations).
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command.</typeparam>
public interface ICommand<TResult>;

/// <summary>
/// Handler for processing commands.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles the command and returns the result.
    /// </summary>
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}
