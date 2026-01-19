namespace SSS.Quality1500.Presentation.Interfaces;

using System.Windows;

public interface IViewService
{
    FrameworkElement GetView(string viewName);

}