using ShellInject;
using ShellInject.Constants;

namespace ShellInjectTests.Navigation;

public class ShellNavigationTests
{
    private class TestPage : ContentPage;

    [Fact]
    public async Task PushAsync_WhenShellIsUnavailable_ShouldThrow()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => ShellNavigation.PushAsync<TestPage>());
        Assert.Equal(ShellInjectConstants.ShellNotFoundText, exception.Message);
    }
}
