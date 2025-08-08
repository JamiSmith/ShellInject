using System.Diagnostics;

namespace Sample.ViewModels;

public partial class FlyoutPageThreeViewModel : BaseViewModel
{
    public override Task DataReceivedAsync(object? parameter)
    {
        Debug.WriteLine(parameter);
        return base.DataReceivedAsync(parameter);
    }
}