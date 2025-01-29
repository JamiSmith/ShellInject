using System.Reflection;
using ShellInject.Constants;
using ShellInject.Interfaces;

namespace ShellInject.Navigation;

/// <summary>
/// Provides navigation methods for Shell-based navigation in a Maui application.
/// </summary>
internal class ShellInjectNavigation
{
    /// <summary>
    /// Provides a singleton instance of the <see cref="ShellInjectNavigation"/> class for Shell-based navigation in a Maui application.
    /// </summary>
    internal static ShellInjectNavigation Instance { get; } = new();

    /// <summary>
    /// Represents the parameter passed during navigation in a Shell-based Maui application.
    /// </summary>
    private object? _navigationParameter;

    /// <summary>
    /// Indicates whether the navigation is being performed in reverse.
    /// </summary>
    private bool _isReverseNavigation;

    /// <summary>
    /// Represents a Shell instance used for navigation in a Maui application.
    /// </summary>
    /// <remarks>
    /// The <see cref="_shell"/> variable holds a reference to the Shell instance, which is used for navigating between pages
    /// in a Shell-based navigation architecture. It is set during the Shell setup process, and should be null when the Shell is not available.
    /// </remarks>
    private Shell? _shell;

    /// <summary>
    /// Sets up the shell for navigation.
    /// </summary>
    /// <param name="shell">The Shell instance to be set up.</param>
    /// <exception cref="NullReferenceException">Thrown if the given shell is null.</exception>
    private void ShellSetup(Shell shell)
    {
        if (shell == null)
        {
            throw new NullReferenceException(ShellInjectConstants.ShellNotFoundText);
        }

        shell.Navigated += OnShellNavigated;
        _shell = shell;
    }

    /// <summary>
    /// Teardown method for Shell navigation.
    /// </summary>
    /// <param name="shell">The Shell instance to teardown.</param>
    private void ShellTeardown(Shell shell)
    {
        shell.Navigated -= OnShellNavigated;
        _shell = null;
    }

    private HashSet<string> GetRegisteredRouteKeys()
    {
        var getRouteKeysMethodInfo = typeof(Routing).GetMethod("GetRouteKeys", BindingFlags.NonPublic | BindingFlags.Static);
        var routeKeyResults = getRouteKeysMethodInfo?.Invoke(null, null);
        if (routeKeyResults is HashSet<string> routeKeys)
        {
            return routeKeys;
        }

        return [];
    }

    /// <summary>
    /// Registers a route for a page type.
    /// </summary>
    /// <param name="pageType">The type of the page to register the route for.</param>
    private string RegisterRoute(Type pageType)
    {
        var prefixedRouteName = $"si_{pageType.Name}";
        var registeredRouteKeys = GetRegisteredRouteKeys();
        if (!registeredRouteKeys.Contains(prefixedRouteName))
        {
            Routing.RegisterRoute(prefixedRouteName, pageType);
        }
        else
        {
            var updatedRoute = $"{prefixedRouteName}_{Guid.NewGuid()}";
            Routing.RegisterRoute(updatedRoute, pageType);
        }

        return prefixedRouteName;
    }

    /// <summary>
    /// Event handler for the Shell.Navigated event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        if ((_shell?.CurrentItem?.CurrentItem as IShellSectionController)?.PresentedPage is ContentPage { BindingContext: IShellInjectShellViewModel viewModel })
        {
            if (_isReverseNavigation)
            {
                viewModel.ReverseDataReceivedAsync(_navigationParameter);
            }
            else
            {
                viewModel.DataReceivedAsync(_navigationParameter);
            }
        }

        _navigationParameter = null;
        _isReverseNavigation = false;
    }

    /// <summary>
    /// Pushes a page onto the navigation stack asynchronously.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter for the page.</typeparam>
    /// <param name="shell">The Shell instance to navigate on.</param>
    /// <param name="pageType">The type of the page to push.</param>
    /// <param name="tParameter">The parameter for the page.</param>
    /// <param name="animate">True to animate the transition, false otherwise. Default is true.</param>
    /// <returns>A Task representing the ongoing asynchronous operation.</returns>
    internal async Task PushAsync<TParameter>(Shell shell, Type pageType, TParameter? tParameter, bool animate = true)
    {
        var route = RegisterRoute(pageType);
        ShellSetup(shell);
        _navigationParameter = tParameter;
        await Shell.Current.GoToAsync(route, animate);
        ShellTeardown(shell);
    }
    
    /// <summary>
    ///  Resets the navigation and replaces the current main page
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="pageType"></param>
    /// <param name="tParameter"></param>
    /// <param name="animate"></param>
    /// <typeparam name="TParameter"></typeparam>
    internal async Task ReplaceAsync<TParameter>(Shell shell, Type? pageType, TParameter? tParameter, bool animate = true)
    {
        if (pageType == null)
        {
            return;
        }

        var isAlreadyCurrentPage = Shell.Current.CurrentPage?.GetType().Name == pageType.Name;
        RegisterRoute(pageType);
        ShellSetup(shell);
        _navigationParameter = tParameter;
        await Shell.Current.Navigation.PopToRootAsync(false);
        await Shell.Current.GoToAsync($"//{pageType.Name}", animate: animate);
        await Task.Delay(500); // awaiting this so the page's binding context has time to set 
        ShellTeardown(shell);

        // If the Page wanting to replace is the same as the current page, then try triggering the DataReceivedAsync method
        // since Shell won't trigger the OnNavigating event in this scenario
        if (isAlreadyCurrentPage)
        {
            if (Shell.Current.CurrentPage is ContentPage { BindingContext: IShellInjectShellViewModel viewModel })
            {
                await viewModel.DataReceivedAsync(tParameter);
            }
        }
    }
    
    /// <summary>
    /// Changes the currently selected tab in a Shell-based Maui application.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter passed during navigation.</typeparam>
    /// <param name="shell">The Shell instance.</param>
    /// <param name="tabIndex">The index of the tab to be selected.</param>
    /// <param name="tParameter">The parameter passed during navigation.</param>
    /// <param name="popToRootFirst">A flag indicating whether to pop to the root before changing the tab.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal async Task ChangeTabAsync<TParameter>(Shell shell, int tabIndex, TParameter? tParameter, bool popToRootFirst)
    {
        ShellSetup(shell);

        if (popToRootFirst)
        {
            await Shell.Current.Navigation.PopToRootAsync(false);
        }
        
        _navigationParameter = tParameter;

        if (Shell.Current.Items[0]?.Items?.Count < tabIndex)
        {
            return;
        }

        var shellSections = Shell.Current.Items[0]?.Items;
        if (shellSections != null)
        {
            var itemTo = shellSections[tabIndex];
            Shell.Current.CurrentItem = itemTo;
        }
        
        ShellTeardown(shell);
    }

    /// <summary>
    /// Asynchronously pops the topmost page from the navigation stack and returns a result value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="shell">The Shell instance to perform the pop operation on.</param>
    /// <param name="tResult">The result value.</param>
    /// <param name="animate">Whether to animate the pop transition. Default is true.</param>
    /// <returns>A task representing the asynchronous pop operation.</returns>
    internal async Task PopAsync<TResult>(Shell shell, TResult tResult, bool animate = true)
    {
        ShellSetup(shell);

        _isReverseNavigation = true;
        _navigationParameter = tResult;
        await Shell.Current.GoToAsync("..", animate);
        
        ShellTeardown(shell);
    }

    /// <summary>
    /// Pops all pages from the navigation stack and returns to the root page.
    /// </summary>
    internal async Task PopToAsync<TResult>(Shell shell, Type pageType, TResult tResult)
    {
        var navigationStack = Shell.Current.Navigation.NavigationStack;

        if (navigationStack is { Count: > 0 })
        {
            // Iterate backwards through the navigation stack to find the target page type
            for (int i = navigationStack.Count - 1; i >= 0; i--)
            {
                var item = navigationStack[i];
            
                // Skip null entries
                if (item != null && item.GetType() == pageType)
                {
                    // Calculate how many pages to pop back (relative navigation)
                    var pagesToPop = navigationStack.Count - 1 - i;

                    // Pop the required number of pages back using relative routing
                    for (int popCount = 0; popCount < pagesToPop; popCount++)
                    {
                        var animate = false;
                        if (popCount == pagesToPop)
                        {
                            ShellSetup(shell);
                            _isReverseNavigation = true;
                            _navigationParameter = tResult;
                            animate = true;
                        }
                        await Shell.Current.GoToAsync("..", animate);
                        ShellTeardown(shell);
                    }
                    
                    return;
                }
            }
        }
        
        // If pageType not found, return to root
        await Shell.Current.GoToAsync("//", true);
    }

    /// <summary>
    /// Pops all pages from the navigation stack and returns to the root page.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="shell">The Shell instance to perform the navigation.</param>
    /// <param name="tResult">The result parameter to be passed during navigation.</param>
    /// <param name="animate">True to animate the navigation, false otherwise. Default is true.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    internal async Task PopToRootAsync<TResult>(Shell shell, TResult tResult, bool animate = true)
    {
        ShellSetup(shell);

        _isReverseNavigation = true;
        _navigationParameter = tResult;
        await Shell.Current.Navigation.PopToRootAsync(animate);
        
        ShellTeardown(shell);
    }

    /// <summary>
    /// Pushes multiple pages onto the Shell navigation stack asynchronously.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter to pass to the pages.</typeparam>
    /// <param name="shell">The Shell instance.</param>
    /// <param name="pageTypes">The list of page types to navigate to.</param>
    /// <param name="tParameter">The parameter to pass to the pages.</param>
    /// <param name="animate">A boolean value indicating whether to animate the navigation.</param>
    /// <param name="animateAllPages">A boolean value indicating whether to animate all pages during the navigation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="NullReferenceException">Thrown when the pageTypes parameter is null or empty.</exception>
    internal async Task PushMultiStackAsync<TParameter>(Shell shell, List<Type> pageTypes, TParameter tParameter, bool animate, bool animateAllPages)
    {
        if (pageTypes == null || pageTypes.Count == 0)
        {
            throw new NullReferenceException(ShellInjectConstants.NavigationStatesExceptionText);
        }

        var lastState = pageTypes.Last();
        foreach (var type in pageTypes)
        {
            var route = RegisterRoute(type);
            if (type == lastState)
            {
                ShellSetup(shell);
                _navigationParameter = tParameter;
            }
            
            await Shell.Current.GoToAsync(route, type == lastState ? animate : animateAllPages);
        }

        ShellTeardown(shell);
    }

    /// <summary>
    /// Pushes a ContentPage as a modal with navigation.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter to be passed to the view model associated with the page. Pass null if no parameter needs to be passed.</typeparam>
    /// <param name="shell">The Shell instance.</param>
    /// <param name="page">The ContentPage to be pushed.</param>
    /// <param name="tParameter">The parameter to be passed to the view model associated with the page.</param>
    /// <param name="animate">Specifies whether the navigation transition should be animated or not. Default value is true.</param>
    /// <exception cref="NullReferenceException">Thrown if the given shell is null or the given page is null.</exception>
    internal async Task PushModalWithNavigation<TParameter>(Shell shell, ContentPage page, TParameter? tParameter, bool animate = true)
    {
        ShellSetup(shell);

        if (page == null)
        {
            throw new NullReferenceException(ShellInjectConstants.NullContentPageExceptionText);
        }

        await Shell.Current.Navigation.PushModalAsync(new NavigationPage(page), animate);
        if (page.BindingContext is IShellInjectShellViewModel vm)
        {
            if (tParameter != null)
            {
                _= vm.DataReceivedAsync(tParameter);
            }
        }
        
        ShellTeardown(shell);
    }
    
    /// <summary>
    /// Pushes a ContentPage as a modal
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="pageType"></param>
    /// <param name="tParameter"></param>
    /// <param name="animate"></param>
    /// <typeparam name="TParameter"></typeparam>
    /// <exception cref="NullReferenceException"></exception>
    internal async Task PushModalAsync(Shell shell, Type pageType, object? tParameter, bool animate = true)
    {
        if (Activator.CreateInstance(pageType) is ContentPage contentPage)
        {
            ShellSetup(shell);

            await Shell.Current.Navigation.PushModalAsync(contentPage, animate);
            if (contentPage.BindingContext is IShellInjectShellViewModel vm)
            {
                if (tParameter != null)
                {
                    _= vm.DataReceivedAsync(tParameter);
                }
            }
        
            ShellTeardown(shell);   
        }
    }
    
    /// <summary>
    /// Looks for the specified Page on the stack and sends the data using the ReverseDataReceivedAsync method
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="page"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    internal Task SendDataToPageAsync(Shell shell, Type? page, object? data = null)
    {
        if (page == null)
        {
            return Task.CompletedTask;
        }
        
        var navigationStack = Shell.Current?.Navigation?.NavigationStack;
        if (navigationStack == null || navigationStack.Count == 0)
        {
            return Task.CompletedTask;
        }

        var pageToSendDataTo = navigationStack
            .Where(p => p != null) 
            .FirstOrDefault(p => p.GetType().Name == page.Name);

        if (pageToSendDataTo is { BindingContext: IShellInjectShellViewModel vm })
        {
            vm.ReverseDataReceivedAsync(data);
        }

        return Task.CompletedTask;
    }
}