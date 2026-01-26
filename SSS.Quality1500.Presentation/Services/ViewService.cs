namespace SSS.Quality1500.Presentation.Services;

using Microsoft.Extensions.DependencyInjection;
using SSS.Quality1500.Presentation.Interfaces;
using SSS.Quality1500.Presentation.Views;
using System.Windows;


public class ViewService(IServiceProvider serviceProvider) : IViewService
{

    public FrameworkElement GetView(string viewName)
    {
        return viewName switch
        {
            "About" => CreateFrameworkElement<AboutUserControl>(),
            "Config" => CreateFrameworkElement<ConfigUserControl>(),
            "Processing" => CreateFrameworkElement<ProcessingUserControl>(),
            _ => throw new ArgumentException($"View {viewName} not found")
        };
    }

    private FrameworkElement CreateFrameworkElement<T>() where T : FrameworkElement
    {
        var view = serviceProvider.GetRequiredService<T>();
        return view;
    }
}