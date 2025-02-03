using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sample.ContentPages;
using ShellInject;

namespace Sample.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty] private string _reverseDataText = string.Empty;
    
    public override Task ReverseDataReceivedAsync(object? parameter)
    {
        if (parameter is string text)
        {
            ReverseDataText = text;
        }
        return Task.CompletedTask;
    }

    public override Task DataReceivedAsync(object? parameter)
    {
        Debug.WriteLine($"DataReceivedAsync: {parameter}");
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnShowDetailsAsync()
    {
        ReverseDataText = string.Empty;
        return Shell.Current.PushAsync(typeof(DetailsPage), "Hello from Main Page!");
    }
    
    [RelayCommand]
    private Task OnPushModalAsync()
    {
        ReverseDataText = string.Empty;
        return Shell.Current.PushModalAsync(typeof(DetailsPage), "This is a modal");
    }
    
    [RelayCommand]
    private Task OnShowPopupAsync()
    {
        ReverseDataText = string.Empty;
        return Shell.Current.ShowPopupAsync<SamplePopup>(null, "This is a modal");
    }
}