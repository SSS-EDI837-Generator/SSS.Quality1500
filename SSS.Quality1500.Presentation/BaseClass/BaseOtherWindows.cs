namespace SSS.Quality1500.Presentation.BaseClass;

using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SSS.Quality1500.Common;
using SSS.Quality1500.Presentation.Interfaces;


public abstract class BaseOtherWindows(ILogger<BaseOtherWindows> logger) : ObservableObject, IMouseDown {
    /// <summary>
    /// Maneja el evento de presionar el botón del mouse para permitir arrastrar la ventana.
    /// </summary>
    public virtual void MouseDown(object sender, MouseButtonEventArgs e) {
        Result<bool, string> result = TryMouseDown(sender, e);
        if (result.IsFailure) {
            logger.LogError("Error en evento MouseDown: {Error}", result.GetErrorOrDefault());
        }
    }

    /// <summary>
    /// Implementación interna que usa Result para manejar el evento MouseDown.
    /// </summary>
    protected virtual Result<bool, string> TryMouseDown(object sender, MouseButtonEventArgs e) {
        try {
            // if (e.ChangedButton != MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            //     return Result<bool, string>.Ok(false);

            if (e.LeftButton != MouseButtonState.Pressed)
                return Result<bool, string>.Ok(false);

            if (sender is not Window window) {
                logger.LogWarning("El sender no es una ventana válida en MouseDown");
                return Result<bool, string>.Fail("El sender no es una ventana válida");
            }

            window.DragMove();
            return Result<bool, string>.Ok(true);

        }
        catch (InvalidOperationException) {
            // Se ignora porque puede ocurrir si la ventana está maximizada
            logger.LogDebug("Intento de DragMove en ventana maximizada");
            return Result<bool, string>.Ok(false);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error al mover la ventana");
            return Result<bool, string>.Fail($"Error al mover la ventana: {ex.Message}");
        }
    }


}