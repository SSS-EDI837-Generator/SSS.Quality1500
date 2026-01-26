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
/// Interfaz para listeners de eventos
/// Principio ISP: Interfaz específica para cada tipo de evento
/// </summary>
/// <typeparam name="TEvent">Tipo del evento a escuchar</typeparam>
public interface IEventListener<in TEvent> where TEvent : IEvent
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
    /// Publica un evento a todos los listeners registrados
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <param name="eventData">Datos del evento</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default) where TEvent : IEvent;

    /// <summary>
    /// Suscribe un listener a un tipo específico de evento
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <param name="listener">Listener del evento</param>
    void Subscribe<TEvent>(IEventListener<TEvent> listener) where TEvent : IEvent;

    /// <summary>
    /// Desuscribe un listener de un tipo específico de evento
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <param name="listener">Listener del evento</param>
    void Unsubscribe<TEvent>(IEventListener<TEvent> listener) where TEvent : IEvent;

    /// <summary>
    /// Obtiene el número de listeners registrados para un tipo de evento
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <returns>Número de listeners registrados</returns>
    int GetListenerCount<TEvent>() where TEvent : IEvent;
}
