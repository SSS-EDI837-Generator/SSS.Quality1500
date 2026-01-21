namespace SSS.Quality1500.Domain.Interfaces;
/// <summary>
/// Interfaz para eventos que pueden ser publicados a través del Event Aggregator
/// Principio ISP: Interfaz específica para eventos
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Timestamp cuando el evento fue creado
    /// </summary>
    DateTime Timestamp { get; }
}

/// <summary>
/// Interfaz para manejadores de eventos
/// Principio ISP: Interfaz específica para cada tipo de evento
/// </summary>
/// <typeparam name="TEvent">Tipo del evento a manejar</typeparam>
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    /// <summary>
    /// Maneja el evento especificado
    /// </summary>
    /// <param name="eventData">Datos del evento</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task Handle(TEvent eventData, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interfaz para el Event Aggregator
/// Implementa el patrón Mediator para desacoplar componentes
/// Principio SRP: Se encarga únicamente de la distribución de eventos
/// Principio OCP: Extensible para nuevos tipos de eventos
/// </summary>
public interface IEventAggregator
{
    /// <summary>
    /// Publica un evento a todos los manejadores registrados
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <param name="eventData">Datos del evento</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default) where TEvent : IEvent;

    /// <summary>
    /// Suscribe un manejador a un tipo específico de evento
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <param name="handler">Manejador del evento</param>
    void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent;

    /// <summary>
    /// Desuscribe un manejador de un tipo específico de evento
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <param name="handler">Manejador del evento</param>
    void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent;

    /// <summary>
    /// Obtiene el número de manejadores registrados para un tipo de evento
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <returns>Número de manejadores registrados</returns>
    int GetHandlerCount<TEvent>() where TEvent : IEvent;
}
