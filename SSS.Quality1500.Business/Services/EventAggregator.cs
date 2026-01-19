namespace SSS.Quality1500.Business.Services;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SSS.Quality1500.Domain.Interfaces;


/// <summary>
/// Implementación del Event Aggregator para manejar eventos de dominio
/// Principio SRP: Se encarga únicamente de la distribución de eventos
/// Principio OCP: Extensible para nuevos tipos de eventos
/// Thread-safe para uso en aplicaciones multi-hilo
/// </summary>
public class EventAggregator : IEventAggregator
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<object>> _handlers;
    private readonly ILogger<EventAggregator> _logger;

    /// <summary>
    /// Constructor primario
    /// </summary>
    /// <param name="logger">Logger para diagnósticos</param>
    public EventAggregator(ILogger<EventAggregator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _handlers = new ConcurrentDictionary<Type, ConcurrentBag<object>>();
    }

    /// <summary>
    /// Publica un evento a todos los manejadores registrados
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <param name="eventData">Datos del evento</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    public Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        if (eventData == null)
        {
            _logger.LogWarning("Intentando publicar evento null de tipo {EventType}", typeof(TEvent).Name);
            return Task.CompletedTask;
        }

        var eventType = typeof(TEvent);
        
        if (!_handlers.TryGetValue(eventType, out var handlersForType))
        {
            _logger.LogDebug("No hay manejadores registrados para el evento {EventType}", eventType.Name);
            return Task.CompletedTask;
        }

        var handlers = handlersForType.OfType<IEventHandler<TEvent>>().ToArray();
        
        if (handlers.Length == 0)
        {
            _logger.LogDebug("No hay manejadores válidos para el evento {EventType}", eventType.Name);
            return Task.CompletedTask;
        }

        _logger.LogTrace("Publicando evento {EventType} a {HandlerCount} manejadores", eventType.Name, handlers.Length);

        // Ejecutar todos los manejadores en paralelo de forma no bloqueante
        _ = Task.Run(async () =>
        {
            var tasks = handlers.Select(handler => HandleEventSafely(handler, eventData, cancellationToken));
            
            try
            {
                await Task.WhenAll(tasks);
                _logger.LogTrace("Evento {EventType} procesado exitosamente por todos los manejadores", eventType.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando evento {EventType}", eventType.Name);
                // No re-lanzar la excepción para no interrumpir el flujo principal
            }
        }, cancellationToken);
        
        // Retornar inmediatamente sin esperar a que terminen los handlers
        _logger.LogTrace("Evento {EventType} enviado a {HandlerCount} manejadores (procesamiento asíncrono)", eventType.Name, handlers.Length);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Suscribe un manejador a un tipo específico de evento
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <param name="handler">Manejador del evento</param>
    public void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        var eventType = typeof(TEvent);
        var handlersForType = _handlers.GetOrAdd(eventType, _ => new ConcurrentBag<object>());
        handlersForType.Add(handler);

        _logger.LogDebug("Manejador {HandlerType} suscrito para evento {EventType}", 
            handler.GetType().Name, eventType.Name);
    }

    /// <summary>
    /// Desuscribe un manejador de un tipo específico de evento
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <param name="handler">Manejador del evento</param>
    public void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        var eventType = typeof(TEvent);
        
        if (_handlers.TryGetValue(eventType, out var handlersForType))
        {
            // Crear nueva colección sin el manejador especificado
            var remainingHandlers = handlersForType.Where(h => !ReferenceEquals(h, handler));
            _handlers.TryUpdate(eventType, new ConcurrentBag<object>(remainingHandlers), handlersForType);
            
            _logger.LogDebug("Manejador {HandlerType} desuscrito del evento {EventType}", 
                handler.GetType().Name, eventType.Name);
        }
    }

    /// <summary>
    /// Obtiene el número de manejadores registrados para un tipo de evento
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <returns>Número de manejadores registrados</returns>
    public int GetHandlerCount<TEvent>() where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        return _handlers.TryGetValue(eventType, out var handlersForType) 
            ? handlersForType.Count 
            : 0;
    }

    /// <summary>
    /// Maneja un evento de forma segura, capturando y registrando excepciones
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento</typeparam>
    /// <param name="handler">Manejador del evento</param>
    /// <param name="eventData">Datos del evento</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    private async Task HandleEventSafely<TEvent>(
        IEventHandler<TEvent> handler, 
        TEvent eventData, 
        CancellationToken cancellationToken) where TEvent : IEvent
    {
        try
        {
            await handler.Handle(eventData, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Manejo de evento {EventType} cancelado para {HandlerType}", 
                typeof(TEvent).Name, handler.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en manejador {HandlerType} para evento {EventType}", 
                handler.GetType().Name, typeof(TEvent).Name);
        }
    }

    /// <summary>
    /// Obtiene estadísticas del Event Aggregator para diagnósticos
    /// </summary>
    /// <returns>Diccionario con estadísticas</returns>
    public Dictionary<string, object> GetStatistics()
    {
        var stats = new Dictionary<string, object>
        {
            ["TotalEventTypes"] = _handlers.Count,
            ["TotalHandlers"] = _handlers.Values.Sum(h => h.Count),
            ["EventTypes"] = _handlers.Keys.Select(k => k.Name).ToArray()
        };

        return stats;
    }
}
