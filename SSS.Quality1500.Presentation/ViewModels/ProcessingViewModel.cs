namespace SSS.Quality1500.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using SSS.Quality1500.Business.Commands.ProcessClaims;
using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Business.Queries.GetImagesFolder;
using SSS.Quality1500.Business.Queries.ValidateDbf;
using Microsoft.Extensions.Options;
using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Collections.ObjectModel;
using System.IO;

/// <summary>
/// ViewModel for the processing view.
/// Allows users to select a DBF file, validate it, and initiate processing.
/// </summary>
public partial class ProcessingViewModel : ObservableObject
{
    private readonly IQueryHandler<GetImagesFolderQuery, Result<string, string>>? _getImagesFolderHandler;
    private readonly IQueryHandler<ValidateDbfQuery, Result<DbfValidationResult, string>>? _validateDbfHandler;
    private readonly ICommandHandler<ProcessClaimsCommand, Result<ClaimProcessingResult, string>>? _processClaimsHandler;
    private readonly IColumnConfigurationRepository? _columnConfigRepository;
    private readonly DbfValidationSettings _dbfSettings = new();

    [ObservableProperty]
    private string _pageTitle = "Procesamiento de Archivos";

    [ObservableProperty]
    private string _selectedPath = string.Empty;

    [ObservableProperty]
    private string _selectedImagesPath = string.Empty;

    [ObservableProperty]
    private string _selectedFileName = string.Empty;

    [ObservableProperty]
    private int _totalImages;

    [ObservableProperty]
    private ObservableCollection<DbfFileInfo> _availableFiles = [];

    [ObservableProperty]
    private DbfFileInfo? _selectedFile;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private bool _isValidating;

    [ObservableProperty]
    private int _progressValue;

    [ObservableProperty]
    private string _statusMessage = "Seleccione una carpeta y archivo DBF para procesar.";

    [ObservableProperty]
    private bool _hasValidSelection;

    [ObservableProperty]
    private bool _isValidated;

    [ObservableProperty]
    private bool _validationPassed;

    [ObservableProperty]
    private int _totalRecords;

    [ObservableProperty]
    private int _totalClaims;

    [ObservableProperty]
    private int _recordsWithErrors;

    [ObservableProperty]
    private bool _showValidationResults;

    [ObservableProperty]
    private bool _showProcessingResults;

    [ObservableProperty]
    private string _validationErrorMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _missingColumns = [];

    /// <summary>
    /// Resultado del procesamiento de claims (para la vista de errores).
    /// </summary>
    [ObservableProperty]
    private ClaimProcessingResult? _lastProcessingResult;

    /// <summary>
    /// Cola de mensajes para el Snackbar de MaterialDesign.
    /// </summary>
    public SnackbarMessageQueue MessageQueue { get; } = new(TimeSpan.FromSeconds(4));

    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public ProcessingViewModel()
    {
        InitializeDefaultPath();
    }

    /// <summary>
    /// Runtime constructor with DI.
    /// </summary>
    public ProcessingViewModel(
        IQueryHandler<GetImagesFolderQuery, Result<string, string>> getImagesFolderHandler,
        IQueryHandler<ValidateDbfQuery, Result<DbfValidationResult, string>> validateDbfHandler,
        ICommandHandler<ProcessClaimsCommand, Result<ClaimProcessingResult, string>> processClaimsHandler,
        IColumnConfigurationRepository columnConfigRepository,
        IOptions<DbfValidationSettings> dbfSettings)
    {
        _getImagesFolderHandler = getImagesFolderHandler;
        _validateDbfHandler = validateDbfHandler;
        _processClaimsHandler = processClaimsHandler;
        _columnConfigRepository = columnConfigRepository;
        _dbfSettings = dbfSettings.Value;
        InitializeDefaultPath();
    }

    private void InitializeDefaultPath()
    {
        string defaultPath = _dbfSettings.DefaultDatabasePath;

        if (string.IsNullOrWhiteSpace(defaultPath) || !Directory.Exists(defaultPath))
            return;

        SelectedPath = defaultPath;
        LoadAvailableFiles();
    }

    partial void OnSelectedPathChanged(string value)
    {
        LoadAvailableFiles();
        ResetValidation();
        ValidateSelection();
    }

    partial void OnSelectedFileChanged(DbfFileInfo? value)
    {
        ResetValidation();

        if (value != null)
            _ = LoadImagesFolderFromDbfAsync(value.FullPath);

        ValidateSelection();
    }

    private async Task LoadImagesFolderFromDbfAsync(string dbfFilePath)
    {
        if (_getImagesFolderHandler == null)
            return;

        try
        {
            GetImagesFolderQuery query = new(dbfFilePath);
            Result<string, string> result = await _getImagesFolderHandler.HandleAsync(query);

            if (!result.IsSuccess)
                return;

            string folderPath = result.GetValueOrDefault()!;

            if (Directory.Exists(folderPath))
                SelectedImagesPath = folderPath;
            else
                MessageQueue.Enqueue($"V0FILEPATH: '{folderPath}' - carpeta no encontrada.");
        }
        catch (Exception ex)
        {
            MessageQueue.Enqueue($"Error al leer ruta de imágenes: {ex.Message}");
        }
    }

    private void ResetValidation()
    {
        IsValidated = false;
        ValidationPassed = false;
        ShowValidationResults = false;
        ShowProcessingResults = false;
        TotalRecords = 0;
        TotalClaims = 0;
        ValidationErrorMessage = string.Empty;
        MissingColumns.Clear();
    }

    private void LoadAvailableFiles()
    {
        AvailableFiles.Clear();

        if (string.IsNullOrEmpty(SelectedPath) || !Directory.Exists(SelectedPath))
            return;

        try
        {
            string[] dbfFiles = Directory.GetFiles(SelectedPath, _dbfSettings.FileFilterPattern, SearchOption.TopDirectoryOnly);

            foreach (string filePath in dbfFiles)
            {
                FileInfo fileInfo = new(filePath);
                AvailableFiles.Add(new DbfFileInfo
                {
                    FileName = fileInfo.Name,
                    FullPath = filePath,
                    FileSize = FormatFileSize(fileInfo.Length),
                    LastModified = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                });
            }

            StatusMessage = AvailableFiles.Count > 0
                ? $"Se encontraron {AvailableFiles.Count} archivos DBF."
                : "No se encontraron archivos DBF en la carpeta seleccionada.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error al leer la carpeta: {ex.Message}";
        }
    }

    private void ValidateSelection()
    {
        HasValidSelection = !string.IsNullOrEmpty(SelectedPath) &&
                           Directory.Exists(SelectedPath) &&
                           !string.IsNullOrEmpty(SelectedImagesPath) &&
                           Directory.Exists(SelectedImagesPath) &&
                           SelectedFile != null;
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    [RelayCommand]
    private void BrowseFolder()
    {
        Microsoft.Win32.OpenFolderDialog dialog = new()
        {
            Title = "Seleccionar carpeta con archivos DBF",
            InitialDirectory = string.IsNullOrEmpty(SelectedPath)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                : SelectedPath
        };

        if (dialog.ShowDialog() == true)
        {
            SelectedPath = dialog.FolderName;
        }
    }

    [RelayCommand]
    private void BrowseImagesFolder()
    {
        Microsoft.Win32.OpenFolderDialog dialog = new()
        {
            Title = "Seleccionar carpeta con imagenes TIF",
            InitialDirectory = string.IsNullOrEmpty(SelectedImagesPath)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                : SelectedImagesPath
        };

        if (dialog.ShowDialog() == true)
        {
            SelectedImagesPath = dialog.FolderName;
            CountTifImages();
        }
    }

    private void CountTifImages()
    {
        TotalImages = 0;

        if (string.IsNullOrEmpty(SelectedImagesPath) || !Directory.Exists(SelectedImagesPath))
            return;

        try
        {
            TotalImages = Directory.GetFiles(SelectedImagesPath, "*.tif", SearchOption.TopDirectoryOnly).Length;
        }
        catch (Exception ex)
        {
            MessageQueue.Enqueue($"Error al contar imagenes: {ex.Message}");
        }
    }

    partial void OnSelectedImagesPathChanged(string value)
    {
        CountTifImages();
        ValidateSelection();
    }

    [RelayCommand]
    private async Task ValidateFileAsync()
    {
        if (SelectedFile == null || _validateDbfHandler == null)
            return;

        IsValidating = true;
        ResetValidation();
        StatusMessage = $"Validando {SelectedFile.FileName}...";

        try
        {
            ValidateDbfQuery query = new(SelectedFile.FullPath);
            Result<DbfValidationResult, string> result = await _validateDbfHandler.HandleAsync(query);

            if (!result.IsSuccess)
            {
                ValidationPassed = false;
                ValidationErrorMessage = result.GetErrorOrDefault() ?? "Error desconocido durante la validación.";
                StatusMessage = $"Error: {ValidationErrorMessage}";
                IsValidated = true;
                ShowValidationResults = true;
                return;
            }

            DbfValidationResult validationResult = result.GetValueOrDefault()!;

            if (!validationResult.IsValid)
            {
                ValidationPassed = false;
                ValidationErrorMessage = validationResult.ErrorMessage ?? "El archivo DBF no es válido.";
                MissingColumns = new ObservableCollection<string>(validationResult.MissingColumns.Take(20));
                StatusMessage = $"Validación fallida: {validationResult.MissingColumns.Count} columnas faltantes.";

                // Write actual columns to file for debugging
                if (validationResult.ActualColumns.Count > 0)
                {
                    string debugPath = Path.Combine(SelectedPath, "dbf_columns_debug.txt");
                    File.WriteAllLines(debugPath, validationResult.ActualColumns);
                    System.Diagnostics.Debug.WriteLine($"Columnas del DBF escritas en: {debugPath}");
                }
            }
            else
            {
                TotalRecords = validationResult.TotalRecords;
                TotalClaims = validationResult.TotalClaims;

                // Validar que total de registros coincida con total de imagenes
                if (TotalRecords != TotalImages)
                {
                    ValidationPassed = false;
                    ValidationErrorMessage = $"El total de registros ({TotalRecords}) no coincide con el total de imagenes TIF ({TotalImages}).";
                    MessageQueue.Enqueue(ValidationErrorMessage);
                    StatusMessage = "Validacion fallida: discrepancia entre registros e imagenes.";
                }
                else
                {
                    ValidationPassed = true;
                    StatusMessage = $"Validacion exitosa. {TotalRecords} registros, {TotalClaims} reclamaciones, {TotalImages} imagenes.";
                }
            }

            IsValidated = true;
            ShowValidationResults = true;
        }
        catch (Exception ex)
        {
            ValidationPassed = false;
            ValidationErrorMessage = $"Error durante la validación: {ex.Message}";
            StatusMessage = ValidationErrorMessage;
            IsValidated = true;
            ShowValidationResults = true;
        }
        finally
        {
            IsValidating = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanProcessFile))]
    private async Task ProcessFileAsync()
    {
        if (SelectedFile == null || _processClaimsHandler == null || _columnConfigRepository == null)
            return;

        IsProcessing = true;
        ProgressValue = 0;
        ShowProcessingResults = false;
        LastProcessingResult = null;
        StatusMessage = $"Procesando {SelectedFile.FileName}...";

        try
        {
            // Load selected columns from configuration
            Result<ColumnConfiguration, string> configResult = _columnConfigRepository.Load();
            List<string> selectedColumns = configResult.IsSuccess && configResult.GetValueOrDefault()?.HasSelectedColumns == true
                ? configResult.GetValueOrDefault()!.SelectedColumns
                : [];

            // Define date and ICD-10 columns to validate
            // TODO: Move to configuration
            List<string> dateColumns =
            [
                "V0FECSER", "V0FECINI", "V0FECFIN", "V0FECNAC",
                "V0FECAUT", "V0FECING", "V0FECALTA"
            ];

            List<string> icd10Columns =
            [
                "V0ICD101", "V0ICD102", "V0ICD103", "V0ICD104",
                "V0ICD105", "V0ICD106", "V0ICD107", "V0ICD108"
            ];

            ProgressValue = 10;

            // Create and execute command
            ProcessClaimsCommand command = new(
                SelectedFile.FullPath,
                SelectedImagesPath,
                selectedColumns,
                dateColumns,
                icd10Columns);

            ProgressValue = 20;
            StatusMessage = "Validando registros...";

            Result<ClaimProcessingResult, string> result = await _processClaimsHandler.HandleAsync(command);

            ProgressValue = 90;

            if (!result.IsSuccess)
            {
                StatusMessage = $"Error: {result.GetErrorOrDefault()}";
                MessageQueue.Enqueue(result.GetErrorOrDefault() ?? "Error desconocido");
                return;
            }

            ClaimProcessingResult processingResult = result.GetValueOrDefault()!;
            LastProcessingResult = processingResult;
            RecordsWithErrors = processingResult.RecordsWithErrors;

            ProgressValue = 100;
            ShowProcessingResults = true;

            if (processingResult.AllRecordsValid)
            {
                StatusMessage = $"Procesamiento completado. Todos los {processingResult.TotalRecords} registros son válidos.";
                MessageQueue.Enqueue("Todos los registros pasaron la validación.");
            }
            else
            {
                StatusMessage = $"Procesamiento completado. {RecordsWithErrors} registros con errores de {processingResult.TotalRecords} totales.";
                MessageQueue.Enqueue($"{RecordsWithErrors} registros requieren revisión.");
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error durante el procesamiento: {ex.Message}";
            MessageQueue.Enqueue($"Error: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private bool CanProcessFile() => HasValidSelection && ValidationPassed && !IsProcessing;

    partial void OnHasValidSelectionChanged(bool value) => ProcessFileCommand.NotifyCanExecuteChanged();
    partial void OnValidationPassedChanged(bool value) => ProcessFileCommand.NotifyCanExecuteChanged();
    partial void OnIsProcessingChanged(bool value) => ProcessFileCommand.NotifyCanExecuteChanged();

    [RelayCommand]
    private void ViewErrors()
    {
        // TODO: Navigate to error review view
        System.Diagnostics.Debug.WriteLine("Navigate to error review view");
    }

    [RelayCommand]
    private void RefreshFiles()
    {
        LoadAvailableFiles();
        ResetValidation();
        SelectedImagesPath = string.Empty;
        TotalImages = 0;
    }
}

/// <summary>
/// Represents a DBF file available for processing.
/// </summary>
public partial class DbfFileInfo : ObservableObject
{
    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _fullPath = string.Empty;

    [ObservableProperty]
    private string _fileSize = string.Empty;

    [ObservableProperty]
    private string _lastModified = string.Empty;
}
