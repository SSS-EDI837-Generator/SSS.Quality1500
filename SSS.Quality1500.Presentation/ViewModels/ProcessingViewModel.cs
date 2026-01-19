namespace SSS.Quality1500.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Reflection;

using System.Collections.Generic;
using System.Text;


public partial class ProcessingViewModel : ObservableObject
{
    [ObservableProperty] private string _labelName = string.Empty;


    public ProcessingViewModel()
    {

        LabelName = "Processing View Model";
    }

}

