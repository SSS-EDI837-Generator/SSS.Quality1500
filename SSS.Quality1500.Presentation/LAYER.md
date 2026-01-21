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
| `ViewModels/` | ViewModels con logica de presentacion | `MainViewModel.cs`, `ControlMainViewModel.cs` |
| `Models/` | ViewModels para binding (pueden usar `ObservableCollection`) | `VkFileRecord.cs` |
| `Services/` | Servicios de UI (navegacion, dialogos) | `NavigationService.cs` |
| `Converters/` | IValueConverter para XAML | `BoolToVisibilityConverter.cs` |
| `Behaviors/` | Behaviors de Interaction | `ScrollIntoViewBehavior.cs` |
| `BaseClass/` | Clases base reutilizables | `ViewModelBase.cs` |
| `Configuration/` | Configuracion de la app | `appsettings.json` |
| `Extensions/` | Extensiones de UI | `VisualTreeExtensions.cs` |
| `Helpers/` | Utilidades de UI | `DialogHelper.cs` |
| `Interfaces/` | Contratos de servicios UI | `IDialogService.cs` |
| `EventHandlers/` | Handlers de eventos globales | `UnhandledExceptionHandler.cs` |

## ViewModels vs DTOs

### ¿Que va en Presentation/Models?
**Solo ViewModels especificos de UI** que necesiten:
- `ObservableCollection<T>` para binding de WPF
- `INotifyPropertyChanged` para actualizaciones de UI
- Propiedades calculadas para presentacion

**Ejemplo:** `VkFileRecord.cs` - ViewModel para mostrar informacion de batch en DataGrid

### ¿Que NO va en Presentation/Models?
❌ **DTOs de negocio** - Esos van en `Business/Models/`
❌ **Entidades de dominio** - Esas van en `Domain/Models/`

### Conversion DTO → ViewModel
```csharp
// Business devuelve List<VdeRecordDto> (DTO)
List<VdeRecordDto> dtos = await vdeService.GetAllAsVdeRecordsAsync("file.dbf");

// Presentation convierte a ObservableCollection para binding
ObservableCollection<VdeRecordDto> viewModels = new(dtos);
```

## Reglas MVVM
1. **NO** code-behind (excepto InitializeComponent)
2. Usar `[ObservableProperty]` y `[RelayCommand]` de CommunityToolkit
3. Binding bidireccional con `UpdateSourceTrigger=PropertyChanged`
4. Navegacion via servicios, no en ViewModels directamente
5. Dialogos via `IDialogService`, no `MessageBox.Show()` (excepto errores criticos)
6. Presentation puede usar `ObservableCollection`, pero Business/Domain NO

## Ejemplo de ViewModel
```csharp
public partial class ControlMainViewModel : ObservableObject
{
    private readonly IVdeRecordService _vdeService;
    
    // ObservableCollection ES VALIDA en Presentation
    [ObservableProperty]
    private ObservableCollection<VkFileRecord> _selectedBatches = new();
    
    [ObservableProperty]
    private bool _isProcessing;
    
    [RelayCommand]
    private async Task StartProcessAsync()
    {
        IsProcessing = true;

        // Business devuelve List<VdeRecordDto> (DTO)
        Result<List<VdeRecordDto>, string> result = await _vdeService.GetAllAsVdeRecordsAsync(filePath);
        
        result.OnSuccess(dtos => 
        {
            // Conversion a ObservableCollection para UI
            SelectedBatches = new ObservableCollection<VkFileRecord>(
                dtos.Select(dto => MapToViewModel(dto))
            );
        });
        
        IsProcessing = false;
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
