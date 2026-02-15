using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sample.Services;
using ShellInject;

namespace Sample.ViewModels;

public partial class DetailsViewModel : BaseViewModel
{
    [ObservableProperty] private string _dataReceivedText = string.Empty;

    public override void OnAppearing()
    {
        base.OnAppearing();
    }

    public override void OnDisAppearing()
    {
        base.OnDisAppearing();
    }

    public override Task OnAppearedAsync()
    {
        return base.OnAppearedAsync();
    }
    
    public override Task DataReceivedAsync(object? parameter)
    {
        if (parameter is string data)
        {
            DataReceivedText = data;
        }
        
        Console.Write("DataReceivedAsync");
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnPopWithParameterAsync()
    {
        return ShellNavigation.PopAsync(parameter: "This text if from Details Page");
    }
}
