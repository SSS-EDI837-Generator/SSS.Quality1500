namespace SSS.Quality1500.Presentation.Models;
using Microsoft.Extensions.Logging;

/// <summary>
/// Builder para crear instancias de LogEntry de forma fluida
/// Implementa el patrón Builder para reducir la cantidad de parámetros del constructor
/// Principio SRP: Se encarga únicamente de construir objetos LogEntry
/// Mantiene la inmutabilidad del objeto LogEntry
/// </summary>
public class LogEntryBuilder
{
    private DateTime _timestamp = DateTime.Now;
    private LogLevel _level = LogLevel.Information;
    private string _category = string.Empty;
    private string _message = string.Empty;
    private Exception? _exception = null;
    private string? _scope = null;

    /// <summary>
    /// Establece el timestamp del log
    /// </summary>
    /// <param name="timestamp">Timestamp del log</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public LogEntryBuilder WithTimestamp(DateTime timestamp)
    {
        _timestamp = timestamp;
        return this;
    }

    /// <summary>
    /// Establece el nivel del log
    /// </summary>
    /// <param name="level">Nivel del log</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public LogEntryBuilder WithLevel(LogLevel level)
    {
        _level = level;
        return this;
    }

    /// <summary>
    /// Establece la categoría del log
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public LogEntryBuilder WithCategory(string category)
    {
        _category = category ?? string.Empty;
        return this;
    }

    /// <summary>
    /// Establece el mensaje del log
    /// </summary>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public LogEntryBuilder WithMessage(string message)
    {
        _message = message ?? string.Empty;
        return this;
    }

    /// <summary>
    /// Establece la excepción del log
    /// </summary>
    /// <param name="exception">Excepción del log</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public LogEntryBuilder WithException(Exception? exception)
    {
        _exception = exception;
        return this;
    }

    /// <summary>
    /// Establece el scope del log
    /// </summary>
    /// <param name="scope">Scope del log</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public LogEntryBuilder WithScope(string? scope)
    {
        _scope = scope;
        return this;
    }

    /// <summary>
    /// Establece nivel, categoría y mensaje en una sola llamada
    /// Método de conveniencia para los datos más comunes
    /// </summary>
    /// <param name="level">Nivel del log</param>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public LogEntryBuilder WithBasicInfo(LogLevel level, string category, string message)
    {
        _level = level;
        _category = category ?? string.Empty;
        _message = message ?? string.Empty;
        return this;
    }

    /// <summary>
    /// Construye la instancia final de LogEntry
    /// </summary>
    /// <returns>Instancia inmutable de LogEntry</returns>
    public LogEntry Build()
    {
        return new LogEntry(
            _timestamp,
            _level,
            _category,
            _message,
            _exception,
            _scope
        );
    }

    /// <summary>
    /// Crea un nuevo builder
    /// </summary>
    /// <returns>Nueva instancia del builder</returns>
    public static LogEntryBuilder Create() => new();

    /// <summary>
    /// Crea un builder con valores básicos pre-configurados
    /// </summary>
    /// <param name="level">Nivel del log</param>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder con valores básicos configurados</returns>
    public static LogEntryBuilder Create(LogLevel level, string category, string message) =>
        new LogEntryBuilder()
            .WithLevel(level)
            .WithCategory(category)
            .WithMessage(message);

    /// <summary>
    /// Crea un builder para log de información
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder configurado para información</returns>
    public static LogEntryBuilder Information(string category, string message) =>
        new LogEntryBuilder()
            .WithLevel(LogLevel.Information)
            .WithCategory(category)
            .WithMessage(message);

    /// <summary>
    /// Crea un builder para log de error
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder configurado para error</returns>
    public static LogEntryBuilder Error(string category, string message) =>
        new LogEntryBuilder()
            .WithLevel(LogLevel.Error)
            .WithCategory(category)
            .WithMessage(message);

    /// <summary>
    /// Crea un builder para log de error con excepción
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <param name="exception">Excepción del log</param>
    /// <returns>Builder configurado para error con excepción</returns>
    public static LogEntryBuilder Error(string category, string message, Exception exception) =>
        new LogEntryBuilder()
            .WithLevel(LogLevel.Error)
            .WithCategory(category)
            .WithMessage(message)
            .WithException(exception);

    /// <summary>
    /// Crea un builder para log de warning
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder configurado para warning</returns>
    public static LogEntryBuilder Warning(string category, string message) =>
        new LogEntryBuilder()
            .WithLevel(LogLevel.Warning)
            .WithCategory(category)
            .WithMessage(message);

    /// <summary>
    /// Crea un builder para log de debug
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder configurado para debug</returns>
    public static LogEntryBuilder Debug(string category, string message) =>
        new LogEntryBuilder()
            .WithLevel(LogLevel.Debug)
            .WithCategory(category)
            .WithMessage(message);
}
