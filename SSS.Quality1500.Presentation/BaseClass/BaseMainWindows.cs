namespace SSS.Quality1500.Presentation.BaseClass;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SSS.Quality1500.Common;
using System.Windows;
using System.Windows.Input;
using SSS.Quality1500.Presentation.Interfaces;
using SSS.Quality1500.Domain.Models;


/// <summary>
/// Clase base abstracta para ventanas principales que proporciona funcionalidad común.
/// </summary>
public abstract class BaseMainWindows : ObservableObject, IClosingWindows, IMouseDown, IKeyPreview
{
    private const string ExitMessage = "!!!Do you really want to exit?";
    private const string ExitTitle = "Exiting...";
    private bool _isClosing;
    protected readonly ILogger<BaseMainWindows> _logger;

    protected BaseMainWindows(ILogger<BaseMainWindows> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Acción que se ejecuta cuando se está cerrando la ventana.
    /// </summary>
    public Action? OnClosing { get; set; }

    /// <summary>
    /// Evento que se dispara cuando se alterna el menú.
    /// </summary>
    public Action<bool>? ToggleMenuEvent { get; set; }

    /// <summary>
    /// Determina si la ventana puede cerrarse mostrando un diálogo de confirmación.
    /// </summary>
    /// <returns>True si el usuario confirma el cierre, False en caso contrario.</returns>
    public virtual bool CanClose()
    {
        if (_isClosing) return true;
        Result<bool, string> result = TryCanClose();
        if (result.IsFailure)
        {
            _logger.LogError("Error al intentar cerrar la ventana: {Error}", result.GetErrorOrDefault());
        }

        return result.IsSuccess && result.GetValueOrDefault();
    }

    /// <summary>
    /// Implementación interna que usa Result para manejar el cierre de la ventana.
    /// </summary>
    protected virtual Result<bool, string> TryCanClose()
    {
        try
        {
            if (_isClosing) return Result<bool, string>.Ok(true);

            _logger.LogInformation("Mostrando diálogo de confirmación de cierre");
            MessageBoxResult response = MessageBox.Show(
                ExitMessage,
                ExitTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);

            bool shouldClose = response == MessageBoxResult.Yes;
            if (shouldClose)
            {
                _isClosing = true;
                _logger.LogInformation("Usuario confirmó el cierre de la aplicación");
            }
            else
            {
                _logger.LogInformation("Usuario canceló el cierre de la aplicación");
            }

            return Result<bool, string>.Ok(shouldClose);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar diálogo de cierre");
            return Result<bool, string>.Fail($"Error al mostrar diálogo de cierre: {ex.Message}");
        }
    }

    /// <summary>
    /// Maneja el evento de presionar el botón del mouse para permitir arrastrar la ventana.
    /// </summary>
    public virtual void MouseDown(object sender, MouseButtonEventArgs e)
    {
        Result<bool, string> result = TryMouseDown(sender, e);
        if (result.IsFailure)
        {
            _logger.LogError("Error en evento MouseDown: {Error}", result.GetErrorOrDefault());
        }
    }

    /// <summary>
    /// Implementación interna que usa Result para manejar el evento MouseDown.
    /// </summary>
    protected virtual Result<bool, string> TryMouseDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (e.ChangedButton != MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
                return Result<bool, string>.Ok(false);

            if (sender is not Window window)
            {
                _logger.LogWarning("El sender no es una ventana válida en MouseDown");
                return Result<bool, string>.Fail("El sender no es una ventana válida");
            }

            window.DragMove();
            return Result<bool, string>.Ok(true);
        }
        catch (InvalidOperationException)
        {
            // Se ignora porque puede ocurrir si la ventana está maximizada
            _logger.LogDebug("Intento de DragMove en ventana maximizada");
            return Result<bool, string>.Ok(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mover la ventana");
            return Result<bool, string>.Fail($"Error al mover la ventana: {ex.Message}");
        }
    }

    /// <summary>
    /// Maneja el evento de presionar una tecla, cerrando la ventana si se presiona Escape.
    /// </summary>
    public virtual void KeyPreviewKeyDown(object sender, KeyEventArgs e)
    {
        Result<bool, string> result = TryKeyPreviewKeyDown(sender, e);
        if (result.IsSuccess && result.GetValueOrDefault() && sender is Window window)
        {
            _logger.LogInformation("Cerrando ventana por tecla Escape");
            window.Close();
        }
        else if (result.IsFailure)
        {
            _logger.LogError("Error en evento KeyPreviewKeyDown: {Error}", result.GetErrorOrDefault());
        }
    }

    /// <summary>
    /// Implementación interna que usa Result para manejar el evento KeyPreviewKeyDown.
    /// </summary>
    protected virtual Result<bool, string> TryKeyPreviewKeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            if (e.Key != Key.Escape)
                return Result<bool, string>.Ok(false);

            if (sender is not Window window)
            {
                _logger.LogWarning("El sender no es una ventana válida en KeyPreviewKeyDown");
                return Result<bool, string>.Fail("El sender no es una ventana válida");
            }

            _isClosing = false; // Reseteamos el estado antes de preguntar
            _logger.LogInformation("Tecla Escape presionada, mostrando diálogo de confirmación");
            return TryCanClose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar tecla Escape");
            return Result<bool, string>.Fail($"Error al cerrar la ventana: {ex.Message}");
        }
    }

    /// <summary>
    /// Método protegido para invocar el evento ToggleMenu de manera segura.
    /// </summary>
    protected virtual void OnToggleMenu(bool isOpen)
    {
        Result<bool, string> result = TryToggleMenu(isOpen);
        if (result.IsFailure)
        {
            _logger.LogError("Error al alternar menú: {Error}", result.GetErrorOrDefault());
        }
    }

    /// <summary>
    /// Implementación interna que usa Result para manejar el evento ToggleMenu.
    /// </summary>
    protected virtual Result<bool, string> TryToggleMenu(bool isOpen)
    {
        try
        {
            _logger.LogInformation("Alternando menú: {IsOpen}", isOpen);
            ToggleMenuEvent?.Invoke(isOpen);
            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al alternar el menú");
            return Result<bool, string>.Fail($"Error al alternar el menú: {ex.Message}");
        }
    }
}