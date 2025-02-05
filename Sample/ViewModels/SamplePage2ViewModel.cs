using CommunityToolkit.Mvvm.Input;
using Sample.ContentPages;
using ShellInject;

namespace Sample.ViewModels;

public partial class SamplePage2ViewModel : BaseViewModel
{
    [RelayCommand]
    private Task OnPushAsync()
    {
        return Shell.Current.PushAsync<SamplePage3>();
    }
}