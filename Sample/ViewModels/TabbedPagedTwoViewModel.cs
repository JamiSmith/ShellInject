using System.Diagnostics;

namespace Sample.ViewModels;

public partial class TabbedPagedTwoViewModel : BaseViewModel
{
    public override Task DataReceivedAsync(object? parameter)
    {
        if (parameter is string data)
        {
            Debug.WriteLine(data);    
        }
        
        return base.DataReceivedAsync(parameter);
    }
}   