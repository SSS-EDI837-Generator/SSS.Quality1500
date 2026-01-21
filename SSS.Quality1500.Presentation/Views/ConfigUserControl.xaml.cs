namespace SSS.Quality1500.Presentation.Views;

using SSS.Quality1500.Presentation.ViewModels;
using System.Windows.Controls;

/// <summary>
/// Interaction logic for ConfigUserControl.xaml
/// </summary>
public partial class ConfigUserControl : UserControl
{
    public ConfigUserControl(ConfigViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

