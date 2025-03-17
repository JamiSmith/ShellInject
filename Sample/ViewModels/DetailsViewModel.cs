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
    }

    public override Task OnPageAppearedAsync()
    {
        return base.OnPageAppearedAsync();
    }

    public override Task OnPageDisAppearingAsync()
    {
        return base.OnPageDisAppearingAsync();
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
        return Shell.Current.PopAsync("This text if from Details Page");
    }
}