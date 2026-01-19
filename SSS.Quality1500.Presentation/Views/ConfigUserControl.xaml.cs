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
}
