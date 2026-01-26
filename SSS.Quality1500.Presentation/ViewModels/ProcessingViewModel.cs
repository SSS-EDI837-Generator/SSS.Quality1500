namespace SSS.Quality1500.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Business.Services.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Collections.ObjectModel;
using System.IO;

/// <summary>
/// ViewModel for the processing view.
/// Allows users to select a DBF file, validate it, and initiate processing.
/// </summary>
public partial class ProcessingViewModel : ObservableObject
{
    private readonly IDbfValidationService? _validationService;

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
    public ProcessingViewModel(IDbfValidationService validationService)
    {
        _validationService = validationService;
        InitializeDefaultPath();
    }

    private void InitializeDefaultPath()
    {
        string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Base Datos");
        if (Directory.Exists(defaultPath))
        {
            SelectedPath = defaultPath;
            LoadAvailableFiles();
        }
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
        ValidateSelection();
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
            string[] dbfFiles = Directory.GetFiles(SelectedPath, "*.DBF", SearchOption.TopDirectoryOnly);

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
        if (SelectedFile == null || _validationService == null)
            return;

        IsValidating = true;
        ResetValidation();
        StatusMessage = $"Validando {SelectedFile.FileName}...";

        try
        {
            Result<DbfValidationResult, string> result = await _validationService.ValidateDbfFileAsync(SelectedFile.FullPath);

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
        if (SelectedFile == null)
            return;

        IsProcessing = true;
        ProgressValue = 0;
        ShowProcessingResults = false;
        StatusMessage = $"Procesando {SelectedFile.FileName}...";

        try
        {
            // Simulate processing - replace with actual DBF processing logic
            for (int i = 0; i <= 100; i += 10)
            {
                await Task.Delay(200);
                ProgressValue = i;
            }

            // Simulated results - replace with actual validation results
            RecordsWithErrors = 3;
            ShowProcessingResults = true;
            StatusMessage = $"Procesamiento completado. {RecordsWithErrors} registros con errores de {TotalRecords} totales.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error durante el procesamiento: {ex.Message}";
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
