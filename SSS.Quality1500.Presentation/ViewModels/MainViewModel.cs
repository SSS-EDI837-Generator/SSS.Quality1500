namespace SSS.Quality1500.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSS.Quality1500.Presentation.Interfaces;
using SSS.Quality1500.Presentation.BaseClass;
using SSS.Quality1500.Presentation.Services;
using SSS.Quality1500.Presentation.Views;
using SSS.Quality1500.Presentation.Models;

public partial class MainViewModel : BaseMainWindowsService
{
    [ObservableProperty] private string _title = "";
    [ObservableProperty] private string _statusMessage = "Ready";
    [ObservableProperty] private object? _currentView;

    private readonly IViewService _viewService;
    private new readonly ILogger<MainViewModel> _logger;
    private readonly ApplicationSettings _appSettings;


    public MainViewModel(
        ILogger<MainViewModel> logger,
        IViewService viewService,
        IOptions<ApplicationSettings> appSettings) : base(logger)
    {
        _logger = logger;
        _viewService = viewService;
        _appSettings = appSettings.Value;

        // Cargar el título desde la configuración
        Title = _appSettings.ApplicationName;

        ShowAbout();
    }

    [RelayCommand]
    private void ShowAbout()
    {
        StatusMessage = "About";
        CurrentView = _viewService.GetView("About");
    }

    [RelayCommand]
    private void ShowConfig()
    {
        StatusMessage = "Config";
        CurrentView = _viewService.GetView("Config");
    }

    [RelayCommand]
    private void ShowProcessing()
    {
        StatusMessage = "Processing";
        CurrentView = _viewService.GetView("Processing");
    }

    [RelayCommand]
    private void Exit()
    {
        OnClosing?.Invoke();
    }
}
