namespace SSS.Quality1500.Presentation.Helpers;

using SSS.Quality1500.Presentation.Interfaces;
using System.Windows;

public class KeyPreviewKeyDown {
    public static bool GetEnableKeyPreviewKeyDown(DependencyObject obj, bool value) {
        return (bool)obj.GetValue(EnableKeyPreviewKeyDownProperty);
    }

    public static void SetEnableKeyPreviewKeyDown(DependencyObject obj, bool value) {
        obj.SetValue(EnableKeyPreviewKeyDownProperty, value);
    }

    public static readonly DependencyProperty EnableKeyPreviewKeyDownProperty =
        DependencyProperty.RegisterAttached(
            "EnableKeyPreviewKeyDown",
            typeof(bool),
            typeof(KeyPreviewKeyDown),
            new PropertyMetadata(false, OnEnableKeyPreviewKeyDownChanged)
        );

    private static void OnEnableKeyPreviewKeyDownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is Window window) {
            window.Loaded += (o, eventArgs) => {
                if (window.DataContext is IKeyPreview vm) {
                    window.PreviewKeyDown += (sender, keyEventArgs) => vm.KeyPreviewKeyDown(sender, keyEventArgs);
                }
            };
        }
    }
}