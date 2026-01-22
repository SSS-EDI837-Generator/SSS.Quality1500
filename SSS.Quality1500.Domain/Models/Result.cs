namespace SSS.Quality1500.Domain.Models;

/// <summary>
/// Representa el resultado de una operacion que puede ser exitosa con un valor de tipo TSuccess
/// o fallida con un error de tipo TFailure.
/// </summary>
/// <typeparam name="TSuccess">Tipo del valor en caso de exito.</typeparam>
/// <typeparam name="TFailure">Tipo del error en caso de fallo.</typeparam>
public abstract class Result<TSuccess, TFailure>
{
    /// <summary>
    /// Representa un resultado exitoso con un valor.
    /// </summary>
    public sealed class Success : Result<TSuccess, TFailure>
    {
        /// <summary>
        /// Obtiene el valor del resultado exitoso.
        /// </summary>
        public TSuccess Value { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="Success"/> con el valor especificado.
        /// </summary>
        /// <param name="value">El valor del resultado exitoso.</param>
        public Success(TSuccess value) => Value = value;
    }

    /// <summary>
    /// Representa un resultado fallido con un error.
    /// </summary>
    public sealed class Failure : Result<TSuccess, TFailure>
    {
        /// <summary>
        /// Obtiene el error del resultado fallido.
        /// </summary>
        public TFailure Error { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="Failure"/> con el error especificado.
        /// </summary>
        /// <param name="error">El error del resultado fallido.</param>
        public Failure(TFailure error) => Error = error;
    }

    /// <summary>
    /// Indica si el resultado es exitoso.
    /// </summary>
    public bool IsSuccess => this is Success;

    /// <summary>
    /// Indica si el resultado es fallido.
    /// </summary>
    public bool IsFailure => this is Failure;

    /// <summary>
    /// Obtiene el valor si el resultado es exitoso, o el valor por defecto si es fallido.
    /// </summary>
    /// <returns>El valor del resultado o default(TSuccess).</returns>
    public TSuccess? GetValueOrDefault()
        => this is Success s ? s.Value : default;

    /// <summary>
    /// Obtiene el error si el resultado es fallido, o el valor por defecto si es exitoso.
    /// </summary>
    /// <returns>El error del resultado o default(TFailure).</returns>
    public TFailure? GetErrorOrDefault()
        => this is Failure f ? f.Error : default;

    /// <summary>
    /// Transforma el valor del resultado exitoso usando la funcion especificada.
    /// </summary>
    /// <typeparam name="TNew">El tipo del nuevo valor.</typeparam>
    /// <param name="mapper">Funcion que transforma el valor.</param>
    /// <returns>Un nuevo Result con el valor transformado o el error original.</returns>
    public Result<TNew, TFailure> Map<TNew>(Func<TSuccess, TNew> mapper)
        => this is Success s
            ? new Result<TNew, TFailure>.Success(mapper(s.Value))
            : new Result<TNew, TFailure>.Failure(((Failure)this).Error);

    /// <summary>
    /// Encadena una operacion que retorna un Result al resultado actual.
    /// </summary>
    /// <typeparam name="TNew">El tipo del valor del nuevo Result.</typeparam>
    /// <param name="binder">Funcion que retorna un nuevo Result.</param>
    /// <returns>El Result de la funcion binder o el error original.</returns>
    public Result<TNew, TFailure> Bind<TNew>(Func<TSuccess, Result<TNew, TFailure>> binder)
        => this is Success s ? binder(s.Value) : new Result<TNew, TFailure>.Failure(((Failure)this).Error);

    /// <summary>
    /// Obtiene el valor si es exitoso, o lanza una excepcion si es fallido.
    /// </summary>
    /// <param name="message">Mensaje opcional para la excepcion.</param>
    /// <returns>El valor del resultado exitoso.</returns>
    /// <exception cref="InvalidOperationException">Si el resultado es fallido.</exception>
    public TSuccess GetValueOrThrow(string? message = null)
        => this is Success s
            ? s.Value
            : throw new InvalidOperationException(message ?? $"El resultado no contiene un valor. Error: {GetErrorOrDefault()}");

    /// <summary>
    /// Ejecuta una accion si el resultado es exitoso.
    /// </summary>
    /// <param name="action">Accion a ejecutar con el valor.</param>
    /// <returns>El mismo Result para encadenar operaciones.</returns>
    public Result<TSuccess, TFailure> OnSuccess(Action<TSuccess> action)
    {
        if (this is Success s)
            action(s.Value);
        return this;
    }

    /// <summary>
    /// Ejecuta una accion si el resultado es fallido.
    /// </summary>
    /// <param name="action">Accion a ejecutar con el error.</param>
    /// <returns>El mismo Result para encadenar operaciones.</returns>
    public Result<TSuccess, TFailure> OnFailure(Action<TFailure> action)
    {
        if (this is Failure f)
            action(f.Error);
        return this;
    }

    /// <summary>
    /// Crea un resultado exitoso con el valor especificado.
    /// </summary>
    /// <param name="value">El valor del resultado.</param>
    /// <returns>Un Result exitoso.</returns>
#pragma warning disable CA1000 // No declarar miembros estáticos en tipos genéricos - Factory methods son más legibles aquí
    public static Result<TSuccess, TFailure> Ok(TSuccess value)
        => new Success(value);

    /// <summary>
    /// Crea un resultado fallido con el error especificado.
    /// </summary>
    /// <param name="error">El error del resultado.</param>
    /// <returns>Un Result fallido.</returns>
    public static Result<TSuccess, TFailure> Fail(TFailure error)
        => new Failure(error);
#pragma warning restore CA1000

    /// <summary>
    /// Ejecuta una de las dos funciones segun el estado del resultado (pattern matching).
    /// </summary>
    /// <typeparam name="TResult">El tipo del valor retornado.</typeparam>
    /// <param name="onSuccess">Funcion a ejecutar si es exitoso.</param>
    /// <param name="onFailure">Funcion a ejecutar si es fallido.</param>
    /// <returns>El resultado de la funcion ejecutada.</returns>
    public TResult Match<TResult>(
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onFailure)
        => this is Success s ? onSuccess(s.Value) : onFailure(((Failure)this).Error);
}
