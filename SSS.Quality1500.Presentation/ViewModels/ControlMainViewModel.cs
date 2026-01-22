namespace SSS.Quality1500.Presentation.ViewModels;

using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using SSS.Quality1500.Presentation.Interfaces;
using SSS.Quality1500.Common.Services;
using SSS.Quality1500.Presentation.Services;
using SSS.Quality1500.Common;
using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Business.Services.Interfaces;
using SSS.Quality1500.Presentation.Models;
using SSS.Quality1500.Domain.Models;

public partial class ControlMainViewModel : ObservableObject, IDisposable
{
    [ObservableProperty] private string _title = "Control de Procesamiento";
    [ObservableProperty] private string _description = "Sistema de procesamiento de claims profesionales";
    [ObservableProperty] private string _status = "Inactivo";
    [ObservableProperty] private Brush _statusColor = Brushes.Gray;
    [ObservableProperty] private string _lastProcessTime = "N/A";
    [ObservableProperty] private string _processedRecords = "0";
    [ObservableProperty] private string _executionTime = "N/A";
    [ObservableProperty] private bool _isProcessing = false;
    [ObservableProperty] private double _progress = 0;
    [ObservableProperty] private string _progressText = "";
    [ObservableProperty] private bool _canStartProcess = true;

    [ObservableProperty] private bool _canStopProcess = false;

    // Token de cancelación para el proceso
    private CancellationTokenSource? _cancellationTokenSource;

    // Control para dispose
    private bool _disposed;

    // Propiedades de configuración
    [ObservableProperty] private string _configurationFiles = "Sin configurar";
    [ObservableProperty] private string _configurationOptions = "Sin configurar";
    [ObservableProperty] private ObservableCollection<VkFileRecord> _selectedBatches = new();
    [ObservableProperty] private ObservableCollection<string> _batchItems = new();

    private readonly ILogger<ControlMainViewModel> _logger;
    private readonly IViewService _viewService;
    private readonly IUiConfigurationService _configurationService;
    private readonly LazyService<IStartQualityProcessRefactored> _startProcessRefactored;
    private readonly ProgressEventHandlerService _progressEventHandlerService;

    /// <summary>
    /// Constructor primario
    /// </summary>
    /// <param name="logger">Logger para diagnósticos</param>
    /// <param name="viewService">Servicio de vistas</param>
    /// <param name="configurationService">Servicio de configuración</param>
    /// <param name="start837ProcessRefactored">Servicio de procesamiento</param>
    /// <param name="progressEventHandlerService">Servicio de manejadores de eventos</param>
    public ControlMainViewModel(
        ILogger<ControlMainViewModel> logger,
        IViewService viewService,
        IUiConfigurationService configurationService,
        LazyService<IStartQualityProcessRefactored> startProcessRefactored,
        ProgressEventHandlerService progressEventHandlerService)
    {

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _startProcessRefactored = startProcessRefactored ?? throw new ArgumentNullException(nameof(startProcessRefactored));
        _progressEventHandlerService = progressEventHandlerService ?? throw new ArgumentNullException(nameof(progressEventHandlerService));

        _logger.LogInformation("ControlViewModel creado con sistema de eventos");
        InitializeStatus();
        InitializeEventHandlers();
        _configurationService.ConfigurationChanged += UpdateConfigurationDisplay;
        UpdateConfigurationDisplay();
    }

    private void InitializeStatus()
    {
        Status = "Listo";
        StatusColor = Brushes.Green;
        LastProcessTime = "Nunca";
        ProcessedRecords = "0";
        ExecutionTime = "0 ms";
    }

    /// <summary>
    /// Inicializa los manejadores de eventos de progreso
    /// </summary>
    private void InitializeEventHandlers()
    {
        try
        {
            // Registrar manejadores de eventos para este ViewModel
            _progressEventHandlerService.RegisterHandlers(this, Application.Current.Dispatcher);
            _logger.LogInformation("Manejadores de eventos de progreso registrados exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inicializando manejadores de eventos de progreso");
        }
    }

    [RelayCommand]
    private async Task StartProcess()
    {
        if (IsProcessing) return;

        IsProcessing = true;
        CanStartProcess = false;
        CanStopProcess = true;
        Status = "Procesando";
        StatusColor = Brushes.Orange;
        Progress = 0;
        ProcessedRecords = "0";
        ProgressText = "Iniciando proceso...";

        // Crear token de cancelación
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            DateTime startTime = DateTime.Now;

            // Obtener configuración del servicio
            DataTable vdeTable = _configurationService.VdeTable;
            string? pathDat = _configurationService.PathOut;
            var fileNameOutput837 = $"TSA837P_{DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}";
            var yy = _configurationService.SelectedDate.Year.ToString();
            ObservableCollection<string> lista = _configurationService.BatchItems;

            // Log información del usuario y sistema
            _logger.LogInformation("Proceso iniciado por usuario: {Username} en máquina: {MachineName} - Dominio: {UserDomainName}",
                Environment.UserName, Environment.MachineName, Environment.UserDomainName);

            // Log configuración completa seleccionada por el usuario
            _logger.LogInformation("Configuración de archivos - VK: {VkFile} | DAT: {DatFile} | IDX: {IdxFile} | IMG_IN: {ImgFileIn} | IMG_OUT: {ImgFileOut} | PATH_OUT: {PathOut}",
                _configurationService.VkFile ?? "No configurado",
                _configurationService.DatFile ?? "No configurado",
                _configurationService.IdxFile ?? "No configurado",
                _configurationService.ImgFileIn ?? "No configurado",
                _configurationService.ImgFileOut ?? "No configurado",
                pathDat ?? "No configurado");

            _logger.LogInformation("Configuración de opciones: {Stamping} | IDA: {Ida} | Zipper: {Zipper} | Conduce: {Conduce} | Backup: {Backup} | ConduceDummy: {ConduceDummy} | Version: {Version} | RemoveHeader: {RemoveHeader} | 837ToXls: {ToXls}",
                _configurationService.Is837StampingChecked,
                _configurationService.IsIdaChecked,
                _configurationService.IsZipperChecked,
                _configurationService.IsConduceChecked,
                _configurationService.IsBackupChecked,
                _configurationService.IsConduceDummyChecked,
                _configurationService.IsVersionChecked,
                _configurationService.IsRemoveHeaderChecked,
                _configurationService.Is837ToXlsChecked);

            _logger.LogInformation("Configuración de procesamiento - Fecha seleccionada: {SelectedDate} | Total batches: {BatchCount} | Total registros VDE: {VdeRecords} | Archivo salida: {OutputFile}",
                _configurationService.SelectedDate.ToString("yyyy-MM-dd"),
                lista.Count,
                vdeTable.Rows.Count,
                fileNameOutput837);

            // Crear el objeto de configuración para el proceso
            // Los eventos de progreso se manejan automáticamente via Event Aggregator
            StartProcessRequest processRequest = new StartProcessRequest(vdeTable, pathDat ?? "", fileNameOutput837)
                .WithYear(yy)
                .WithSelectedBatches(lista)
                .WithCancellationToken(_cancellationTokenSource.Token);

            // Proceso real con progreso y cancelación
            // Los eventos de progreso se publican automáticamente y actualizan la UI
            Result<ProcessingResult, string> resultProcess = await _startProcessRefactored.Value.StartProcess(processRequest);

            // Manejar resultado - el progreso ya se actualizó via eventos
            if (resultProcess.IsFailure)
            {
                string errorMessage = resultProcess.GetErrorOrDefault() ?? "Error desconocido";

                // Actualizar estado UI para errores
                Status = "Error";
                StatusColor = Brushes.Red;
                ProgressText = "Proceso terminado con errores";

                // Determinar el tipo de error y respuesta apropiada
                if (errorMessage.Contains("cancelado"))
                {
                    // Error de cancelación - solo log, estado ya actualizado por eventos
                    _logger.LogInformation("Proceso cancelado detectado en ViewModel");
                }
                else if (errorMessage.Contains("Error crítico"))
                {
                    // Error crítico - siempre mostrar MessageBox detallado y detener proceso
                    _logger.LogError("Error crítico detectado: {ErrorMessage}", errorMessage);

                    MessageBox.Show(
                        $"Se ha producido un error crítico que impide continuar el procesamiento:\n\n{errorMessage}\n\nEl proceso ha sido detenido. Revise los logs para más detalles y corrija el problema antes de intentar nuevamente.",
                        "Error Crítico - Procesamiento Detenido",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );

                }
                else
                {
                    // Otros errores - mostrar MessageBox estándar
                    MessageBox.Show(
                        $"Error durante el procesamiento:\n\n{errorMessage}\n\nRevise los logs para más información.",
                        "Error en el procesamiento",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
                return;
            }

            // Logging adicional para proceso completado
            ProcessingResult? result = resultProcess.GetValueOrDefault();
            if (result != null)
            {
                _logger.LogInformation("Proceso completado: {FilesGenerated} archivos generados, {TotalClaims} claims procesados, {TotalServiceLines} líneas de servicio",
                    result.FilesGenerated, result.TotalClaims, result.TotalServiceLines);
            }
        }
        catch (OperationCanceledException)
        {
            // El estado se actualiza via eventos, solo log local
            _logger.LogInformation("Proceso cancelado por el usuario");
            MessageBox.Show("Proceso cancelado por el usuario", "Proceso cancelado por el usuario", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            // Los eventos manejan el estado, solo mostrar error crítico
            _logger.LogError(ex, "Error inesperado durante el procesamiento");
            MessageBox.Show($"Error crítico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsProcessing = false;
            CanStartProcess = true;
            CanStopProcess = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            // Reset después de 3 segundos solo si el proceso no fue cancelado o tuvo error
            if (Status == "Completado")
            {
                await Task.Delay(3000);
                if (!IsProcessing)
                {
                    Progress = 0;
                    ProgressText = "";
                    Status = "Listo";
                    StatusColor = Brushes.Green;
                }
            }
        }
    }

    [RelayCommand]
    private void StopProcess()
    {
        if (!IsProcessing || _cancellationTokenSource == null) return;

        _logger.LogInformation("Proceso 837P detenido por usuario: {Username} en máquina: {MachineName} - Timestamp: {Timestamp}",
            Environment.UserName, Environment.MachineName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        _cancellationTokenSource.Cancel();
        ProgressText = "Cancelando proceso...";
    }

    [RelayCommand]
    private void ViewLogs()
    {
        try
        {
            _logger.LogInformation("Visor de logs abierto por usuario: {Username} en máquina: {MachineName} - Timestamp: {Timestamp}",
                Environment.UserName, Environment.MachineName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            // Usar el servicio de vista para abrir la ventana de logs
            var logViewerWindow = _viewService.GetView("LogViewerWindow") as Window;

            if (logViewerWindow != null)
            {
                logViewerWindow.Show();
            }
            else
            {
                _logger.LogError("No se pudo obtener la ventana LogViewerWindow del servicio de vista");
                MessageBox.Show(
                    "Error abriendo el visor de logs. Por favor, contacte al administrador.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error abriendo el visor de logs");
            MessageBox.Show(
                $"Error abriendo el visor de logs: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    [RelayCommand]
    private void Configure()
    {
        // Abrir la ventana de configuración modal usando DI
        _logger.LogInformation("Open ConfigurationWindow");
        // ✅ MEJORADO: Usar servicio inyectado
        _configurationService.ClearAllConfiguration();
        var configWindow = _viewService.GetView("ConfigurationWindow") as Window;
        configWindow?.ShowDialog();
    }

    [RelayCommand]
    private void ClearConfiguration()
    {
        var result = MessageBox.Show(
            "¿Está seguro de que desea limpiar toda la configuración?",
            "Confirmar Limpieza",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            // ✅ MEJORADO: Usar servicio inyectado
            _configurationService.ClearAllConfiguration();
            MessageBox.Show(
                "Configuración limpiada exitosamente.",
                "Configuración Limpiada",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

    private void UpdateConfigurationDisplay()
    {
        _logger.LogInformation("UpdateConfigurationDisplay llamado");
        IUiConfigurationService config = _configurationService;

        // Actualizar archivos configurados
        var files = new System.Collections.Generic.List<string>();
        if (!string.IsNullOrEmpty(config.VkFile)) files.Add($"VK: {config.VkFile}");
        if (!string.IsNullOrEmpty(config.DatFile)) files.Add($"DAT: {config.DatFile}");
        if (!string.IsNullOrEmpty(config.IdxFile)) files.Add($"IDX: {config.IdxFile}");
        if (!string.IsNullOrEmpty(config.ImgFileIn)) files.Add($"IMG IN: {config.ImgFileIn}");
        if (!string.IsNullOrEmpty(config.ImgFileOut)) files.Add($"IMG OUT: {config.ImgFileOut}");

        ConfigurationFiles = files.Count > 0 ? string.Join("\n", files) : "Sin archivos configurados";

        // Actualizar opciones configuradas
        var options = new System.Collections.Generic.List<string>();
        if (config.Is837StampingChecked) options.Add("837 Stamping");
        if (config.IsIdaChecked) options.Add("IDA");
        if (config.IsZipperChecked) options.Add("Zipper");
        if (config.IsConduceChecked) options.Add("Conduce");
        if (config.IsBackupChecked) options.Add("Backup");
        if (config.IsConduceDummyChecked) options.Add("Conduce Dummy");
        if (config.IsVersionChecked) options.Add("VDVERSION");
        if (config.IsRemoveHeaderChecked) options.Add("Remove Header");
        if (config.Is837ToXlsChecked) options.Add("837 to Xls");

        ConfigurationOptions = options.Count > 0 ? string.Join(", ", options) : "Sin opciones seleccionadas";

        _logger.LogInformation("Actualizando BatchItems, items en ConfigurationService: {Count}", config.BatchItems.Count);

        SelectedBatches.Clear();
        BatchItems.Clear();

        foreach (string batch in config.BatchItems)
        {
            BatchItems.Add(batch);
            _logger.LogDebug("Agregado batch a BatchItems: {Batch}", batch);

            string[] split = batch.Split('|');
            if (split.Length >= 4)
            {
                var record = new VkFileRecord
                {
                    BatchFrom = split[0],
                    BatchTo = split[1],
                    Date = split[2],
                    Documents = split[3] == "True",
                };
                record.RecordId++;
                SelectedBatches.Add(record);
            }
        }

        _logger.LogInformation("BatchItems actualizado, total items: {Count}", BatchItems.Count);

        // Actualizar disponibilidad del botón de inicio
        CanStartProcess = config.IsConfigurationComplete() && !IsProcessing;
    }

    // Override para actualizar la disponibilidad del botón cuando cambia IsProcessing
    partial void OnIsProcessingChanged(bool value)
    {
        // ✅ MEJORADO: Usar servicio inyectado
        CanStartProcess = _configurationService.IsConfigurationComplete() && !value;
    }

    /// <summary>
    /// Método público para forzar actualización desde otros ViewModels
    /// </summary>
    public void ForceUpdateConfiguration()
    {
        _logger.LogInformation("ForceUpdateConfiguration llamado manualmente");
        UpdateConfigurationDisplay();
    }

    /// <summary>
    /// Libera los recursos utilizados por el ViewModel
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            try
            {
                _logger.LogInformation("Liberando recursos del Control837PViewModel");

                // Desregistrar manejadores de eventos
                _progressEventHandlerService?.Dispose();

                // Desuscribir eventos de configuración
                if (_configurationService != null)
                {
                    _configurationService.ConfigurationChanged -= UpdateConfigurationDisplay;
                }

                // Cancelar cualquier proceso en curso
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();

                _disposed = true;
                _logger.LogInformation("Control837PViewModel disposed correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante dispose de Control837PViewModel");
            }
        }
    }
}
