namespace SSS.Quality1500.Presentation.Views;

using SSS.Quality1500.Presentation.ViewModels;
using System.Windows.Controls;

public partial class Icd10UserControl : UserControl
{
    public Icd10UserControl(Icd10ViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
