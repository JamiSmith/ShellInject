using CommunityToolkit.Mvvm.Input;
using Sample.ContentPages;
using ShellInject;

namespace Sample.ViewModels;

public partial class SamplePage3ViewModel : BaseViewModel
{
    [RelayCommand]
    private Task OnCloseAsync()
    {
        return Shell.Current.PopModalStackAsync("Modal Stack Popped");
    }
    
    [RelayCommand]
    private Task OnPushAsync()
    {
        return Shell.Current.PushAsync<SamplePage2>();
    }
}