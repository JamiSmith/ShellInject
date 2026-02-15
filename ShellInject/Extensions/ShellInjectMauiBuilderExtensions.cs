using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using Microsoft.Maui.LifecycleEvents;
using ShellInject.Interfaces;
using ShellInject.Services;

namespace ShellInject;

/// <summary>
/// Contains extension methods for configuring ShellInject in a Maui app.
/// </summary>
public static class ShellInjectMauiBuilderExtensions
{
    private static readonly SemaphoreSlim StartupNavigationSemaphore = new(1, 1);
    private static EventHandler<ShellNavigatedEventArgs>? _navigatedHandler;
    private static Shell? _trackedShell;
    private static bool _startupNavigationHandled;
    private static bool _windowTrackingInitialized;

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
            life.AddiOS(i => i.FinishedLaunching((app, launchOptions) =>
            {
                InitializeWindowTracking();
                return true;
            }));
#endif
#if ANDROID 
            life.AddAndroid(a => a.OnCreate((activity, state) =>
            {
                InitializeWindowTracking();
            }));
#endif
        });

        
        return builder;
    }
    
    private static void InitializeWindowTracking()
    {
        if (_windowTrackingInitialized)
        {
            return;
        }

        if (Application.Current is not { } app)
        {
            return;
        }

        _windowTrackingInitialized = true;
        app.PropertyChanged += OnApplicationPropertyChanged;
        if (app.Windows is INotifyCollectionChanged windowNotifier)
        {
            windowNotifier.CollectionChanged += OnWindowsCollectionChanged;
        }

        TryAttachShell(GetRootPage(app));
    }

    private static void OnApplicationPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Application.Windows))
        {
            return;
        }

        if (Application.Current is not { } app)
        {
            return;
        }

        TryAttachShell(GetRootPage(app));
    }

    private static void OnWindowsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (Application.Current is not { } app)
        {
            return;
        }

        TryAttachShell(GetRootPage(app));
    }

    private static Page? GetRootPage(Application app)
    {
        if (app.Windows.Count == 0)
        {
            return null;
        }

        return app.Windows[0].Page;
    }

    private static void TryAttachShell(Page? rootPage)
    {
        if (rootPage is Shell shell)
        {
            AttachShell(shell);
            return;
        }

        DetachShell();
    }

    private static void AttachShell(Shell shell)
    {
        if (ReferenceEquals(_trackedShell, shell))
        {
            if (!_startupNavigationHandled && _navigatedHandler is null)
            {
                AttachShellNavigatedHandler(shell);
                _ = TryHandleInitialNavigationAsync(shell);
            }

            return;
        }

        DetachShell();
        _trackedShell = shell;
        _startupNavigationHandled = false;
        AttachShellNavigatedHandler(shell);
        _ = TryHandleInitialNavigationAsync(shell);
    }

    private static void AttachShellNavigatedHandler(Shell shell)
    {
        _navigatedHandler = async void (s, e) =>
        {
            if (s is not Shell navigatedShell)
            {
                return;
            }

            await TryHandleInitialNavigationAsync(navigatedShell);
        };

        shell.Navigated += _navigatedHandler;
    }

    private static void DetachShell()
    {
        if (_trackedShell is not null && _navigatedHandler is not null)
        {
            _trackedShell.Navigated -= _navigatedHandler;
        }

        _trackedShell = null;
        _navigatedHandler = null;
        _startupNavigationHandled = false;
    }

    private static void DetachShellNavigatedHandler(Shell shell)
    {
        if (_navigatedHandler is null)
        {
            return;
        }

        shell.Navigated -= _navigatedHandler;
        _navigatedHandler = null;
    }

    private static async Task TryHandleInitialNavigationAsync(Shell shell)
    {
        if (_startupNavigationHandled || !ReferenceEquals(_trackedShell, shell))
        {
            return;
        }

        await StartupNavigationSemaphore.WaitAsync();
        try
        {
            if (_startupNavigationHandled || !ReferenceEquals(_trackedShell, shell))
            {
                return;
            }

            if (await OnShellNavigatedAsync(shell))
            {
                _startupNavigationHandled = true;
                DetachShellNavigatedHandler(shell);
            }
        }
        catch
        {
            // just catch it
        }
        finally
        {
            StartupNavigationSemaphore.Release();
        }
    }

    private static async Task<bool> OnShellNavigatedAsync(Shell shell)
    {
        var firstPage = shell.CurrentPage;
        if (firstPage?.BindingContext is not IShellInjectShellViewModel vm)
        {
            return false;
        }

        await vm.OnAppearedAsync();

        if (!vm.IsInitialized)
        {
            await vm.InitializedAsync();
            vm.IsInitialized = true;
        }

        return true;
    }
}
