namespace SSS.Quality1500.Domain.Models;
/// <summary>
/// Representa el resultado de una operacion que puede ser exitosa con un valor de tipo TSuccess
/// o fallida con un error de tipo TFailure.
/// </summary>
public abstract class Result<TSuccess, TFailure>
{
    public sealed class Success : Result<TSuccess, TFailure>
    {
        public TSuccess Value { get; }
        public Success(TSuccess value) => Value = value;
    }

    public sealed class Failure : Result<TSuccess, TFailure>
    {
        public TFailure Error { get; }
        public Failure(TFailure error) => Error = error;
    }

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public TSuccess? GetValueOrDefault()
        => this is Success s ? s.Value : default;

    public TFailure? GetErrorOrDefault()
        => this is Failure f ? f.Error : default;

    public Result<TNew, TFailure> Map<TNew>(Func<TSuccess, TNew> mapper)
        => this is Success s
            ? new Result<TNew, TFailure>.Success(mapper(s.Value))
            : new Result<TNew, TFailure>.Failure(((Failure)this).Error);

    public Result<TNew, TFailure> Bind<TNew>(Func<TSuccess, Result<TNew, TFailure>> binder)
        => this is Success s ? binder(s.Value) : new Result<TNew, TFailure>.Failure(((Failure)this).Error);

    public TSuccess GetValueOrThrow(string? message = null)
        => this is Success s
            ? s.Value
            : throw new InvalidOperationException(message ?? $"El resultado no contiene un valor. Error: {GetErrorOrDefault()}");

    public Result<TSuccess, TFailure> OnSuccess(Action<TSuccess> action)
    {
        if (this is Success s)
            action(s.Value);
        return this;
    }

    public Result<TSuccess, TFailure> OnFailure(Action<TFailure> action)
    {
        if (this is Failure f)
            action(f.Error);
        return this;
    }

    public static Result<TSuccess, TFailure> Ok(TSuccess value)
        => new Success(value);

    public static Result<TSuccess, TFailure> Fail(TFailure error)
        => new Failure(error);

    public TResult Match<TResult>(
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onFailure)
        => this is Success s ? onSuccess(s.Value) : onFailure(((Failure)this).Error);
}
