namespace SSS.Quality1500.Presentation.Views
{
    using SSS.Quality1500.Presentation.ViewModels;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ProcessingUserControl.xaml
    /// </summary>
    public partial class ProcessingUserControl : UserControl
    {
        public ProcessingUserControl(ProcessingViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

        }
    }
}
