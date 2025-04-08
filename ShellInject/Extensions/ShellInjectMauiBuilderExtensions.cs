using Microsoft.Maui.LifecycleEvents;
using ShellInject.Interfaces;
using ShellInject.Services;

namespace ShellInject;

/// <summary>
/// Contains extension methods for configuring ShellInject in a Maui app.
/// </summary>
public static class ShellInjectMauiBuilderExtensions
{
    private static EventHandler<ShellNavigatedEventArgs>? _navigatedHandler;

    /// <summary>
    /// Uses ShellInject to configure the ShellInjectMauiBuilderExtensions in a Maui app.
    /// </summary>
    /// <param name="builder">The MauiAppBuilder instance.</param>
    /// <returns>The modified MauiAppBuilder instance.</returns>
    public static MauiAppBuilder UseShellInject(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IMauiInitializeService, ShellInjectInitializer>();
        
        builder.ConfigureLifecycleEvents(life =>
        {
#if IOS
            // Sets up a temporary event to trigger the OnPageAppearedAsync when FinishedLaunching happens in AppDelegate
            life.AddiOS(i => i.FinishedLaunching((app, launchOptions) =>
            {
                if (Application.Current?.MainPage is not Shell shell)
                {
                    return true;
                }
                
                // shell.Navigated += OnShellNavigated;
                _navigatedHandler = async void (s, e) =>
                {
                    try
                    {
                        await OnShellNavigatedAsync(s, e);
                    }
                    catch
                    {
                        // just catch it
                    }
                };
                shell.Navigated += _navigatedHandler;
                return true;
            }));
#endif
#if ANDROID 
            // Sets up a temporary event to trigger the OnPageAppearedAsync when OnCreate happens in android activity

            life.AddAndroid(a => a.OnCreate((activity, state) =>
            {
                if (Application.Current?.MainPage is not Shell shell)
                {
                    return;
                }
                
                // shell.Navigated += OnShellNavigated;
                _navigatedHandler = async void (s, e) =>
                {
                    try
                    {
                        await OnShellNavigatedAsync(s, e);
                    }
                    catch
                    {
                        // just catch it
                    }
                };
                shell.Navigated += _navigatedHandler;
            }));
#endif
        });

        
        return builder;
    }
    
    private static async Task OnShellNavigatedAsync(object? sender, ShellNavigatedEventArgs e)
    {
        if (sender is not Shell shell)
        {
            return;
        }
    
        var firstPage = shell.CurrentPage;
        if (firstPage?.BindingContext is IShellInjectShellViewModel vm)
        {
            await vm.OnAppearedAsync();
            
            if (!vm.IsInitialized)
            {
                await vm.InitializedAsync();
                vm.IsInitialized = true;
            }
        }
    
        // Only need this once, detach the event
        if (_navigatedHandler is not null)
        {
            shell.Navigated -= _navigatedHandler;
        }
    }
}