using CommunityToolkit.Mvvm.Input;
using Sample.ContentPages;
using ShellInject;

namespace Sample.ViewModels;

public partial class SamplePage3ViewModel : BaseViewModel
{
    [RelayCommand]
    private Task OnCloseAsync()
    {
        return ShellNavigation.PopModalStackAsync(data: "Modal Stack Popped");
    }
    
    [RelayCommand]
    private Task OnPushAsync()
    {
        return ShellNavigation.PushAsync<SamplePage2>();
    }
}
