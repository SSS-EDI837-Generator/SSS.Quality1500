namespace SSS.Quality1500.Presentation.Views;

using SSS.Quality1500.Presentation.ViewModels;
using System.Windows.Controls;

/// <summary>
/// Interaction logic for AboutUserControl.xaml
/// </summary>
public partial class AboutUserControl : UserControl
{
    public AboutUserControl(AboutViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

