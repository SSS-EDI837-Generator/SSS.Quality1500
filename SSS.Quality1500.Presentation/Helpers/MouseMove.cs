namespace SSS.Quality1500.Presentation.Helpers;

using System.Windows;
using SSS.Quality1500.Presentation.Interfaces;

public class MouseMove
{
    public static bool GetEnableMouseMove(DependencyObject obj)
    {
        return (bool)obj.GetValue(EnableMouseMoveProperty);
    }

    public static void SetEnableMouseMove(DependencyObject obj, bool value)
    {
        obj.SetValue(EnableMouseMoveProperty, value);
    }

    public static readonly DependencyProperty EnableMouseMoveProperty =
        DependencyProperty.RegisterAttached(
            "EnableMouseMove",
            typeof(bool),
            typeof(MouseMove),
            new PropertyMetadata(false, OnEnableMouseMoveChanged)
        );

    private static void OnEnableMouseMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Window window && (bool)e.NewValue)
        {
            window.Loaded += (o, eventArgs) =>
            {
                if (window.DataContext is IMouseDown mouseDown)
                {
                    window.MouseDown += (sender, buttonEventArgs) => { mouseDown.MouseDown(sender, buttonEventArgs); };
                }
            };
        }
    }
}