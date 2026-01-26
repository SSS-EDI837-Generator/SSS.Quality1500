namespace SSS.Quality1500.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;

/// <summary>
/// ViewModel for the processing view.
/// Allows users to select a DBF file path and initiate processing.
/// </summary>
public partial class ProcessingViewModel : ObservableObject
{
    [ObservableProperty]
    private string _pageTitle = "Procesamiento de Archivos";

    [ObservableProperty]
    private string _selectedPath = string.Empty;

    [ObservableProperty]
    private string _selectedFileName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<DbfFileInfo> _availableFiles = [];

    [ObservableProperty]
    private DbfFileInfo? _selectedFile;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private int _progressValue;

    [ObservableProperty]
    private string _statusMessage = "Seleccione una carpeta y archivo DBF para procesar.";

    [ObservableProperty]
    private bool _hasValidSelection;

    [ObservableProperty]
    private int _totalRecords;

    [ObservableProperty]
    private int _recordsWithErrors;

    [ObservableProperty]
    private bool _showResults;

    public ProcessingViewModel()
    {
        // Set default path if exists
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
        ValidateSelection();
    }

    partial void OnSelectedFileChanged(DbfFileInfo? value)
    {
        ValidateSelection();
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
        // Using Microsoft.Win32.OpenFolderDialog (available in .NET 8+)
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
    private async Task ProcessFileAsync()
    {
        if (SelectedFile == null)
            return;

        IsProcessing = true;
        ProgressValue = 0;
        ShowResults = false;
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
            TotalRecords = 14;
            RecordsWithErrors = 3;
            ShowResults = true;
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
