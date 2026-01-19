namespace SSS.Quality1500.Presentation.Views
{
    using SSS.Quality1500.Presentation.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

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
}
