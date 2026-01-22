namespace SSS.Quality1500.Presentation.Helpers;

using System.Windows;
using SSS.Quality1500.Presentation.Interfaces;

public class WindowCloser
{
    public static bool GetEnableWindowClosing(DependencyObject obj, bool value)
    {
        return (bool)obj.GetValue(EnableWindowClosingProperty);
    }

    public static void SetEnableWindowClosing(DependencyObject obj, bool value)
    {
        obj.SetValue(EnableWindowClosingProperty, value);
    }

    public static readonly DependencyProperty EnableWindowClosingProperty =
        DependencyProperty.RegisterAttached(
            "EnableWindowClosing",
            typeof(bool),
            typeof(WindowCloser),
            new PropertyMetadata(false, OnEnableWindowClosingChanged)
        );

    private static void OnEnableWindowClosingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Window window)
        {
            window.Loaded += (o, eventArgs) =>
            {
                if (window.DataContext is not IClosingWindows vm) return;
                vm.OnClosing += window.Close;
                window.Closing += (sender, args) => { args.Cancel = !vm.CanClose(); };
            };
        }
    }
}