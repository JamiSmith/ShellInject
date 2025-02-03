using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sample.ContentPages;
using ShellInject;

namespace Sample.ViewModels;

public partial class SamplePopupViewModel : BaseViewModel
{
    [ObservableProperty] private string _message = string.Empty;
    
    public override Task DataReceivedAsync(object? parameter)
    {
        if (parameter is not string message)
        {
            return Task.CompletedTask;
        }
        
        Message = message;
        
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnDismissAsync()
    {
        return Shell.Current.DismissPopupAsync<SamplePopup>("Popup Text results");
    }
}