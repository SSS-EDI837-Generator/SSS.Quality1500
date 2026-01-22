namespace SSS.Quality1500.Presentation.Interfaces;

public interface IClosingWindows
{
    Action? OnClosing { get; set; }
    bool CanClose();
}