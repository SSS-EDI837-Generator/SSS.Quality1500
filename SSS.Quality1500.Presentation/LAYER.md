# Presentation Layer (WPF + MVVM)

## Proposito
**Interfaz de usuario** implementada con WPF siguiendo el patron MVVM estricto (sin code-behind).

## Dependencias
- Depende de **Business** (casos de uso)
- Depende de **Domain** (entidades para binding)
- Depende de **Common** (utilidades)
- Usa CommunityToolkit.Mvvm, MaterialDesignThemes, Behaviors

## Estructura de Carpetas

| Carpeta | Contenido | Ejemplo |
|---------|-----------|--------|
| `Views/` | Ventanas y UserControls (XAML) | `MainWindow.xaml` |
| `ViewModels/` | ViewModels con logica de presentacion | `MainViewModel.cs` |
| `Models/` | Modelos de vista (wrappers) | `ClaimDisplayModel.cs` |
| `Services/` | Servicios de UI (navegacion, dialogos) | `NavigationService.cs` |
| `Converters/` | IValueConverter para XAML | `BoolToVisibilityConverter.cs` |
| `Behaviors/` | Behaviors de Interaction | `ScrollIntoViewBehavior.cs` |
| `BaseClass/` | Clases base reutilizables | `ViewModelBase.cs` |
| `Configuration/` | Configuracion de la app | `appsettings.json` |
| `Extensions/` | Extensiones de UI | `VisualTreeExtensions.cs` |
| `Helpers/` | Utilidades de UI | `DialogHelper.cs` |
| `Interfaces/` | Contratos de servicios UI | `IDialogService.cs` |
| `EventHandlers/` | Handlers de eventos globales | `UnhandledExceptionHandler.cs` |

## Reglas MVVM
1. **NO** code-behind (excepto InitializeComponent)
2. Usar `[ObservableProperty]` y `[RelayCommand]` de CommunityToolkit
3. Binding bidireccional con `UpdateSourceTrigger=PropertyChanged`
4. Navegacion via servicios, no en ViewModels directamente
5. Dialogos via `IDialogService`, no `MessageBox.Show()`

## Ejemplo de ViewModel
```csharp
public partial class MainViewModel : ObservableObject
{
    private readonly ClaimLoadingService _claimService;
    
    [ObservableProperty]
    private ObservableCollection<ClaimRecord> _claims = [];
    
    [ObservableProperty]
    private bool _isLoading;
    
    [RelayCommand]
    private async Task LoadClaimsAsync()
    {
        IsLoading = true;
        var result = await _claimService.LoadClaimsAsync("path.dbf");
        result.OnSuccess(data => Claims = new(data));
        IsLoading = false;
    }
}
```

## Ejemplo de View (XAML)
```xml
<Window xmlns:vm="clr-namespace:SSS.Project.Presentation.ViewModels">
    <Window.DataContext>
        <vm:MainViewModel />
    </Window.DataContext>
    
    <Button Command="{Binding LoadClaimsCommand}" Content="Cargar" />
    <DataGrid ItemsSource="{Binding Claims}" />
</Window>
```
