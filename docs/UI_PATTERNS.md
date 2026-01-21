# UI_PATTERNS.md

## Patrones de UI para Revisión Visual de Claims

Este documento describe los patrones de UI específicos para la vista de revisión de errores con zoom en imagen del formulario CMS-1500.

## Vista de Revisión de Errores

### Estructura de la Vista

```
┌─────────────────────────────────────────────────────────┐
│  ErrorReviewView                                        │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌───────────────────────────────────────────────────┐ │
│  │                                                   │ │
│  │         CMS-1500 Form Image                      │ │
│  │         (ZoomableImageControl)                   │ │
│  │                                                   │ │
│  │         [Imagen escaneada con zoom dinámico]     │ │
│  │                                                   │ │
│  └───────────────────────────────────────────────────┘ │
│                                                         │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌───────────────┬─────────────────┬─────────────────┐ │
│  │ NPI:          │ Patient Name:   │ DOS:            │ │
│  │ [1234567890]  │ [Smith, John]   │ [01/15/2025]    │ │
│  └───────────────┴─────────────────┴─────────────────┘ │
│                                                         │
│  [< Previous]  [Accept]  [Correct]  [Next >]          │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

## ViewModel

### ErrorReviewViewModel (Presentation/ViewModels/)

```csharp
namespace SSS.Quality1500.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSS.Quality1500.Domain.Models;
using SSS.Quality1500.Business.Services;

public partial class ErrorReviewViewModel : ObservableObject
{
    private readonly IImageService _imageService;
    private readonly IValidationService _validationService;
    private readonly ILogger<ErrorReviewViewModel> _logger;
    
    [ObservableProperty]
    private ClaimRecord? _currentClaim;
    
    [ObservableProperty]
    private BitmapImage? _formImage;
    
    [ObservableProperty]
    private double _zoomLevel = 1.0;
    
    [ObservableProperty]
    private Rect _focusArea;
    
    [ObservableProperty]
    private string _selectedFieldName = string.Empty;
    
    [ObservableProperty]
    private List<ValidationError> _validationErrors = new();
    
    [ObservableProperty]
    private int _currentErrorIndex;
    
    public ErrorReviewViewModel(
        IImageService imageService,
        IValidationService validationService,
        ILogger<ErrorReviewViewModel> logger)
    {
        _imageService = imageService;
        _validationService = validationService;
        _logger = logger;
    }
    
    /// <summary>
    /// Carga un claim para revisión
    /// </summary>
    [RelayCommand]
    private async Task LoadClaimAsync(string claimId)
    {
        try
        {
            // Cargar claim desde servicio (Business layer)
            Result<ClaimRecord, string> claimResult = 
                await _validationService.GetClaimByIdAsync(claimId);
            
            await claimResult.Match(
                success: async claim =>
                {
                    CurrentClaim = claim;
                    
                    // Cargar imagen del formulario
                    Result<BitmapImage, string> imageResult = 
                        await _imageService.LoadImageAsync(claim.ImagePath);
                    
                    imageResult.OnSuccess(image => FormImage = image);
                    
                    // Cargar errores de validación
                    Result<List<ValidationError>, string> errorsResult = 
                        await _validationService.GetValidationErrorsAsync(claimId);
                    
                    errorsResult.OnSuccess(errors => ValidationErrors = errors);
                },
                failure: error =>
                {
                    _logger.LogError("Failed to load claim {ClaimId}: {Error}", claimId, error);
                    return Task.CompletedTask;
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading claim {ClaimId}", claimId);
        }
    }
    
    /// <summary>
    /// Hace zoom a un campo específico cuando el usuario lo selecciona
    /// </summary>
    [RelayCommand]
    private void ZoomToField(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName) || FormImage == null)
            return;
        
        SelectedFieldName = fieldName;
        
        // Obtener coordenadas del campo en el formulario
        Rect fieldArea = _imageService.GetFieldCoordinates(fieldName);
        
        if (fieldArea != Rect.Empty)
        {
            FocusArea = fieldArea;
            ZoomLevel = CalculateZoomLevel(fieldArea);
            
            _logger.LogInformation("Zoomed to field {FieldName}", fieldName);
        }
    }
    
    /// <summary>
    /// Navega al siguiente error
    /// </summary>
    [RelayCommand]
    private void NextError()
    {
        if (ValidationErrors.Count == 0)
            return;
        
        CurrentErrorIndex = (CurrentErrorIndex + 1) % ValidationErrors.Count;
        ValidationError currentError = ValidationErrors[CurrentErrorIndex];
        
        // Zoom al campo con error
        ZoomToField(currentError.FieldName);
    }
    
    /// <summary>
    /// Navega al error anterior
    /// </summary>
    [RelayCommand]
    private void PreviousError()
    {
        if (ValidationErrors.Count == 0)
            return;
        
        CurrentErrorIndex = CurrentErrorIndex == 0 
            ? ValidationErrors.Count - 1 
            : CurrentErrorIndex - 1;
            
        ValidationError currentError = ValidationErrors[CurrentErrorIndex];
        ZoomToField(currentError.FieldName);
    }
    
    /// <summary>
    /// Marca el error actual como aceptado (excepción válida)
    /// </summary>
    [RelayCommand]
    private async Task AcceptErrorAsync()
    {
        if (CurrentClaim == null || ValidationErrors.Count == 0)
            return;
        
        ValidationError currentError = ValidationErrors[CurrentErrorIndex];
        
        Result<bool, string> result = await _validationService
            .AcceptErrorAsExceptionAsync(CurrentClaim.ClaimId, currentError.RuleCode);
        
        result.OnSuccess(_ =>
        {
            ValidationErrors.RemoveAt(CurrentErrorIndex);
            
            if (ValidationErrors.Count == 0)
            {
                _logger.LogInformation("All errors reviewed for claim {ClaimId}", CurrentClaim.ClaimId);
                // Navegar a siguiente claim o cerrar vista
            }
            else
            {
                NextError();
            }
        });
    }
    
    /// <summary>
    /// Permite corregir el valor del campo
    /// </summary>
    [RelayCommand]
    private async Task CorrectFieldAsync()
    {
        if (CurrentClaim == null || ValidationErrors.Count == 0)
            return;
        
        ValidationError currentError = ValidationErrors[CurrentErrorIndex];
        
        // Abrir diálogo para corrección
        // Implementar con IDialogService
        
        _logger.LogInformation("Opening correction dialog for field {FieldName}", 
            currentError.FieldName);
    }
    
    private double CalculateZoomLevel(Rect fieldArea)
    {
        // Calcular zoom apropiado basado en tamaño del área
        // Por ejemplo, zoom para que el área ocupe ~40% del viewport
        const double targetSizeRatio = 0.4;
        
        return Math.Min(
            targetSizeRatio / (fieldArea.Width / FormImage!.PixelWidth),
            targetSizeRatio / (fieldArea.Height / FormImage.PixelHeight)
        );
    }
}
```

## Custom Control: ZoomableImageControl

### ZoomableImageControl.xaml

```xml
<UserControl x:Class="SSS.Quality1500.Presentation.Controls.ZoomableImageControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
             mc:Ignorable="d" 
             d:DesignHeight="450" d:DesignWidth="800">
    
    <Border BorderBrush="{DynamicResource PrimaryHueLightBrush}"
            BorderThickness="1">
        
        <ScrollViewer x:Name="ImageScrollViewer"
                      HorizontalScrollBarVisibility="Auto"
                      VerticalScrollBarVisibility="Auto"
                      PreviewMouseWheel="OnMouseWheel">
            
            <Grid>
                <!-- Imagen principal -->
                <Image x:Name="MainImage"
                       Source="{Binding ImageSource, RelativeSource={RelativeSource AncestorType=UserControl}}"
                       Stretch="None"
                       RenderTransformOrigin="0.5,0.5">
                    <Image.RenderTransform>
                        <TransformGroup>
                            <ScaleTransform x:Name="ScaleTransform"
                                          ScaleX="{Binding ZoomLevel, RelativeSource={RelativeSource AncestorType=UserControl}}"
                                          ScaleY="{Binding ZoomLevel, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
                            <TranslateTransform x:Name="TranslateTransform"/>
                        </TransformGroup>
                    </Image.RenderTransform>
                </Image>
                
                <!-- Highlight del área seleccionada -->
                <Rectangle x:Name="FocusHighlight"
                           Stroke="{DynamicResource SecondaryAccentBrush}"
                           StrokeThickness="3"
                           Fill="Transparent"
                           Visibility="Collapsed"
                           IsHitTestVisible="False">
                    <Rectangle.RenderTransform>
                        <TransformGroup>
                            <ScaleTransform ScaleX="{Binding ZoomLevel, RelativeSource={RelativeSource AncestorType=UserControl}}"
                                          ScaleY="{Binding ZoomLevel, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
                            <TranslateTransform x:Name="HighlightTranslate"/>
                        </TransformGroup>
                    </Rectangle.RenderTransform>
                </Rectangle>
            </Grid>
        </ScrollViewer>
    </Border>
</UserControl>
```

### ZoomableImageControl.xaml.cs

```csharp
namespace SSS.Quality1500.Presentation.Controls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

public partial class ZoomableImageControl : UserControl
{
    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(
            nameof(ImageSource),
            typeof(BitmapImage),
            typeof(ZoomableImageControl),
            new PropertyMetadata(null));
    
    public static readonly DependencyProperty ZoomLevelProperty =
        DependencyProperty.Register(
            nameof(ZoomLevel),
            typeof(double),
            typeof(ZoomableImageControl),
            new PropertyMetadata(1.0, OnZoomLevelChanged));
    
    public static readonly DependencyProperty FocusAreaProperty =
        DependencyProperty.Register(
            nameof(FocusArea),
            typeof(Rect),
            typeof(ZoomableImageControl),
            new PropertyMetadata(Rect.Empty, OnFocusAreaChanged));
    
    public BitmapImage? ImageSource
    {
        get => (BitmapImage?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }
    
    public double ZoomLevel
    {
        get => (double)GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, value);
    }
    
    public Rect FocusArea
    {
        get => (Rect)GetValue(FocusAreaProperty);
        set => SetValue(FocusAreaProperty, value);
    }
    
    public ZoomableImageControl()
    {
        InitializeComponent();
        
        // Permitir pan con mouse
        MainImage.MouseLeftButtonDown += OnMouseLeftButtonDown;
        MainImage.MouseMove += OnMouseMove;
        MainImage.MouseLeftButtonUp += OnMouseLeftButtonUp;
    }
    
    private Point _mouseDownPosition;
    private bool _isDragging;
    
    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Zoom con mouse wheel
        double zoomDelta = e.Delta > 0 ? 1.1 : 0.9;
        double newZoom = Math.Clamp(ZoomLevel * zoomDelta, 0.5, 5.0);
        
        ZoomLevel = newZoom;
        
        e.Handled = true;
    }
    
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _mouseDownPosition = e.GetPosition(ImageScrollViewer);
        MainImage.CaptureMouse();
    }
    
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging)
            return;
        
        Point currentPosition = e.GetPosition(ImageScrollViewer);
        Vector delta = currentPosition - _mouseDownPosition;
        
        // Pan la imagen
        ImageScrollViewer.ScrollToHorizontalOffset(
            ImageScrollViewer.HorizontalOffset - delta.X);
        ImageScrollViewer.ScrollToVerticalOffset(
            ImageScrollViewer.VerticalOffset - delta.Y);
        
        _mouseDownPosition = currentPosition;
    }
    
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        MainImage.ReleaseMouseCapture();
    }
    
    private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ZoomableImageControl control)
            return;
        
        // Animar zoom suavemente
        DoubleAnimation animation = new()
        {
            To = (double)e.NewValue,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        
        control.ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
        control.ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
    }
    
    private static void OnFocusAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ZoomableImageControl control || e.NewValue is not Rect focusArea)
            return;
        
        if (focusArea == Rect.Empty)
        {
            control.FocusHighlight.Visibility = Visibility.Collapsed;
            return;
        }
        
        // Mostrar y posicionar highlight
        control.FocusHighlight.Visibility = Visibility.Visible;
        control.FocusHighlight.Width = focusArea.Width;
        control.FocusHighlight.Height = focusArea.Height;
        
        control.HighlightTranslate.X = focusArea.X;
        control.HighlightTranslate.Y = focusArea.Y;
        
        // Scroll para centrar el área
        control.ScrollToArea(focusArea);
    }
    
    private void ScrollToArea(Rect area)
    {
        // Calcular posición de scroll para centrar el área
        double centerX = (area.X + area.Width / 2) * ZoomLevel;
        double centerY = (area.Y + area.Height / 2) * ZoomLevel;
        
        double targetScrollX = centerX - ImageScrollViewer.ViewportWidth / 2;
        double targetScrollY = centerY - ImageScrollViewer.ViewportHeight / 2;
        
        // Animar scroll
        DoubleAnimation scrollXAnimation = new()
        {
            To = Math.Max(0, targetScrollX),
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };
        
        DoubleAnimation scrollYAnimation = new()
        {
            To = Math.Max(0, targetScrollY),
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };
        
        // WPF ScrollViewer no soporta animación directa, usar timer
        AnimateScroll(targetScrollX, targetScrollY);
    }
    
    private void AnimateScroll(double targetX, double targetY)
    {
        const int steps = 20;
        int currentStep = 0;
        
        double startX = ImageScrollViewer.HorizontalOffset;
        double startY = ImageScrollViewer.VerticalOffset;
        
        System.Windows.Threading.DispatcherTimer timer = new()
        {
            Interval = TimeSpan.FromMilliseconds(15)
        };
        
        timer.Tick += (s, e) =>
        {
            currentStep++;
            double progress = (double)currentStep / steps;
            
            // Easing
            double easedProgress = 1 - Math.Pow(1 - progress, 3);
            
            double newX = startX + (targetX - startX) * easedProgress;
            double newY = startY + (targetY - startY) * easedProgress;
            
            ImageScrollViewer.ScrollToHorizontalOffset(newX);
            ImageScrollViewer.ScrollToVerticalOffset(newY);
            
            if (currentStep >= steps)
            {
                timer.Stop();
            }
        };
        
        timer.Start();
    }
}
```

## Servicio de Coordenadas de Campos

### IImageService (Domain/Interfaces/)

```csharp
namespace SSS.Quality1500.Domain.Interfaces;

public interface IImageService
{
    Task<Result<BitmapImage, string>> LoadImageAsync(string imagePath);
    Rect GetFieldCoordinates(string fieldName);
}
```

### ImageService (Business/Services/)

```csharp
namespace SSS.Quality1500.Business.Services;

using SSS.Quality1500.Domain.Interfaces;
using System.Windows;
using System.Windows.Media.Imaging;

public sealed class ImageService : IImageService
{
    private readonly Dictionary<string, Rect> _fieldCoordinates;
    private readonly ILogger<ImageService> _logger;
    
    public ImageService(ILogger<ImageService> logger)
    {
        _logger = logger;
        _fieldCoordinates = InitializeFieldCoordinates();
    }
    
    public async Task<Result<BitmapImage, string>> LoadImageAsync(string imagePath)
    {
        try
        {
            if (!File.Exists(imagePath))
            {
                return Result<BitmapImage, string>.Fail($"Image file not found: {imagePath}");
            }
            
            BitmapImage image = new();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(imagePath, UriKind.Absolute);
            image.EndInit();
            image.Freeze(); // Para uso en múltiples threads
            
            _logger.LogInformation("Loaded image from {Path}", imagePath);
            
            return Result<BitmapImage, string>.Ok(image);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load image from {Path}", imagePath);
            return Result<BitmapImage, string>.Fail($"Error loading image: {ex.Message}");
        }
    }
    
    public Rect GetFieldCoordinates(string fieldName)
    {
        if (_fieldCoordinates.TryGetValue(fieldName, out Rect coordinates))
        {
            return coordinates;
        }
        
        _logger.LogWarning("Coordinates not found for field {FieldName}", fieldName);
        return Rect.Empty;
    }
    
    private Dictionary<string, Rect> InitializeFieldCoordinates()
    {
        // Coordenadas basadas en formulario CMS-1500 estándar (8.5" x 11" @ 300 DPI)
        // X, Y, Width, Height en pixels
        return new Dictionary<string, Rect>
        {
            // Box 1 - Insurance Type (arriba izquierda)
            { "InsuranceType", new Rect(150, 200, 600, 50) },
            
            // Box 2 - Patient Name
            { "PatientName", new Rect(150, 300, 800, 50) },
            
            // Box 3 - Patient DOB
            { "PatientDOB", new Rect(150, 400, 300, 50) },
            
            // Box 5 - Patient Address
            { "PatientAddress", new Rect(150, 500, 800, 100) },
            
            // Box 17b - Referring Provider NPI
            { "ReferringNpi", new Rect(1800, 1100, 400, 50) },
            
            // Box 21 - Diagnosis Codes (área completa)
            { "DiagnosisCodes", new Rect(150, 1200, 2000, 200) },
            
            // Box 24A - Date of Service
            { "DateOfService", new Rect(150, 1500, 300, 50) },
            
            // Box 24B - Place of Service
            { "PlaceOfService", new Rect(500, 1500, 150, 50) },
            
            // Box 24D - Procedure Code
            { "ProcedureCode", new Rect(700, 1500, 250, 50) },
            
            // Box 24E - Diagnosis Pointer
            { "DiagnosisPointer", new Rect(1000, 1500, 150, 50) },
            
            // Box 24F - Charges
            { "Charges", new Rect(1200, 1500, 200, 50) },
            
            // Box 24J - Rendering Provider NPI
            { "RenderingNpi", new Rect(1800, 1500, 400, 50) },
            
            // Box 33a - Billing Provider NPI
            { "BillingNpi", new Rect(1800, 2200, 400, 50) }
        };
    }
}
```

## Vista XAML

### ErrorReviewView.xaml

```xml
<UserControl x:Class="SSS.Quality1500.Presentation.Views.ErrorReviewView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:controls="clr-namespace:SSS.Quality1500.Presentation.Controls"
             xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes"
             DataContext="{Binding Source={StaticResource Locator}, Path=ErrorReviewViewModel}">
    
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="2*"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        
        <!-- Imagen del formulario -->
        <controls:ZoomableImageControl Grid.Row="0"
                                      ImageSource="{Binding FormImage}"
                                      ZoomLevel="{Binding ZoomLevel}"
                                      FocusArea="{Binding FocusArea}"
                                      Margin="10"/>
        
        <!-- Panel de campos -->
        <Grid Grid.Row="1" Margin="10">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>
            
            <!-- Error actual -->
            <TextBlock Grid.Row="0"
                       Text="{Binding ValidationErrors[CurrentErrorIndex].Message}"
                       Style="{StaticResource MaterialDesignHeadline6TextBlock}"
                       Foreground="{DynamicResource PrimaryHueMidBrush}"
                       Margin="0,0,0,10"/>
            
            <!-- Campos de datos -->
            <UniformGrid Grid.Row="1" Columns="3" Rows="2">
                <StackPanel Margin="5">
                    <TextBlock Text="NPI:" Style="{StaticResource MaterialDesignCaptionTextBlock}"/>
                    <TextBox Text="{Binding CurrentClaim.Provider.RenderingNpi}"
                             GotFocus="OnFieldGotFocus"
                             Tag="RenderingNpi"/>
                </StackPanel>
                
                <StackPanel Margin="5">
                    <TextBlock Text="Patient Name:" Style="{StaticResource MaterialDesignCaptionTextBlock}"/>
                    <TextBox Text="{Binding CurrentClaim.Patient.FullName}"
                             GotFocus="OnFieldGotFocus"
                             Tag="PatientName"/>
                </StackPanel>
                
                <StackPanel Margin="5">
                    <TextBlock Text="Date of Service:" Style="{StaticResource MaterialDesignCaptionTextBlock}"/>
                    <DatePicker SelectedDate="{Binding CurrentClaim.Service.DateFrom}"
                                GotFocus="OnFieldGotFocus"
                                Tag="DateOfService"/>
                </StackPanel>
                
                <!-- Más campos según necesidad -->
            </UniformGrid>
            
            <!-- Botones de acción -->
            <StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Center" Margin="0,10,0,0">
                <Button Content="Previous" Command="{Binding PreviousErrorCommand}" Margin="5"/>
                <Button Content="Accept" Command="{Binding AcceptErrorCommand}" Margin="5"/>
                <Button Content="Correct" Command="{Binding CorrectFieldCommand}" Margin="5"/>
                <Button Content="Next" Command="{Binding NextErrorCommand}" Margin="5"/>
            </StackPanel>
        </Grid>
    </Grid>
</UserControl>
```

### ErrorReviewView.xaml.cs (Code-Behind Mínimo)

```csharp
namespace SSS.Quality1500.Presentation.Views;

public partial class ErrorReviewView : UserControl
{
    public ErrorReviewView()
    {
        InitializeComponent();
    }
    
    private void OnFieldGotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element && 
            element.Tag is string fieldName &&
            DataContext is ErrorReviewViewModel viewModel)
        {
            viewModel.ZoomToFieldCommand.Execute(fieldName);
        }
    }
}
```
