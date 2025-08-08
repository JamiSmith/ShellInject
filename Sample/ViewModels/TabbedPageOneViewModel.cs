using CommunityToolkit.Mvvm.Input;
using ShellInject;

namespace Sample.ViewModels;

public partial class TabbedPageOneViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task ShowTabTwo()
    {
        await Shell.Current.ChangeTabAsync(1, "Tab Swapped");
    }
}