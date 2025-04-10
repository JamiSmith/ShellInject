using ShellInject.Navigation;

namespace ShellInjectTests.Navigation;

public class RoutingTests : BaseNavigationTests
{
    [Fact]
    public void BuildRoute_ShouldReturnExpectedRoute()
    {
        var nav = new ShellInjectNavigation(); 
        var pageType = typeof(TestPage);
        var result = nav.RegisterRoute(pageType);
        var expectedResult = $"si_{pageType.Name}";
        Assert.Equal(expectedResult, result); 
    }
}