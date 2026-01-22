namespace SSS.Quality1500.Presentation.Services;

using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using SSS.Quality1500.Common;
using SSS.Quality1500.Presentation.BaseClass;


public class BaseMainWindowsService : BaseMainWindows
{
    protected BaseMainWindowsService(ILogger<BaseMainWindows> logger) : base(logger) { }
} // end class