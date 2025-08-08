using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sample.ContentPages;
using Sample.Services;
using ShellInject;

namespace Sample.ViewModels;

public partial class MainViewModel(ISampleService sampleService) : BaseViewModel
{
    [ObservableProperty] private string _reverseDataText = string.Empty;

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

    public override Task InitializedAsync()
    {
        return base.InitializedAsync();
    }

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
        return Shell.Current.PushAsync<DetailsPage>("Hello from Main Page!");
    }
    
    [RelayCommand]
    private Task OnPushModalAsync()
    {
        ReverseDataText = string.Empty;
        return Shell.Current.PushModalAsync<DetailsPage>("This is a modal");
    }
    
    [RelayCommand]
    private async Task OnShowPopupAsync()
    {
        ReverseDataText = string.Empty;
        await Shell.Current.ShowPopupAsync<SamplePopup>("This is a Popup and this text is also from parameter passing using ShellInject.");
    }
    
    [RelayCommand]
    private Task OnNavigateTestAsync()
    {
        ReverseDataText = string.Empty;
        return Shell.Current.PushModalWithNavigationAsync(new SamplePage2());
    }

    [RelayCommand]
    private async Task ReplaceContent()
    {
        await Shell.Current.ReplaceAsync<FlyoutPageThree>("Content Replaced");
    }
}