namespace SSS.Quality1500.Presentation.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public partial class ConfigViewModel : ObservableObject
    {
        [ObservableProperty] private string _labelName = string.Empty;
        
        public ConfigViewModel()
        {
            LabelName = "Config View Model";
        }

    }
}
