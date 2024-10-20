using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShellInject;

namespace Sample.ViewModels;

public partial class DetailsViewModel : BaseViewModel
{
    [ObservableProperty] private string _dataReceivedText = string.Empty;

    public override void OnAppearing()
    {
        base.OnAppearing();
        
        Debug.WriteLine("OnAppearing");
    }

    public override void OnDisAppearing()
    {
        base.OnDisAppearing();
        
        Debug.WriteLine("OnDisAppearing");
    }

    public override Task DataReceivedAsync(object? parameter)
    {
        if (parameter is string data)
        {
            DataReceivedText = data;
        }
        
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnPopWithParameterAsync()
    {
        return Shell.Current.PopAsync("This text if from Details Page");
    }
}