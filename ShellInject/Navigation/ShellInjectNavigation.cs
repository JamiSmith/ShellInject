using System.Reflection;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using ShellInject.Constants;
using ShellInject.Interfaces;

namespace ShellInject.Navigation;

/// <summary>
/// Provides navigation methods for Shell-based navigation in a Maui application.
/// </summary>
internal class ShellInjectNavigation : IShellInjectNavigation
{
    private static readonly List<Popup> PopupStack = [];
    
    /// <summary>
    /// Provides a singleton instance of the <see cref="ShellInjectNavigation"/> class for Shell-based navigation in a Maui application.
    /// </summary>
    internal static ShellInjectNavigation Instance { get; } = new();
    
    public virtual EventHandler<ShellNavigatedEventArgs>? NavigatedHandler { get; set; }

    /// <summary>
    /// Represents the parameter passed during navigation in a Shell-based Maui application.
    /// </summary>
    public virtual object? NavigationParameter { get; set; }

    /// <summary>
    /// Indicates whether the navigation is being performed in reverse.
    /// </summary>
    public virtual bool IsReverseNavigation { get; set; }

    /// <summary>
    /// Represents a Shell instance used for navigation in a Maui application.
    /// </summary>
    /// <remarks>
    /// The <see cref="Shell"/> variable holds a reference to the Shell instance, which is used for navigating between pages
    /// in a Shell-based navigation architecture. It is set during the Shell setup process, and should be null when the Shell is not available.
    /// </remarks>
    public virtual Shell? Shell { get; set; }

    /// <summary>
    /// Sets up the shell for navigation.
    /// </summary>
    /// <param name="shell">The Shell instance to be set up.</param>
    /// <param name="addNavigatedHandler"></param>
    /// <exception cref="NullReferenceException">Thrown if the given shell is null.</exception>
    private void ShellSetup(Shell shell, bool addNavigatedHandler = true)
    {
        if (shell == null)
        {
            throw new NullReferenceException(ShellInjectConstants.ShellNotFoundText);
        }

        if (addNavigatedHandler)
        {
            NavigatedHandler = async void (s, e) =>
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
            shell.Navigated += NavigatedHandler;
        }

        Shell = shell;
    }

    /// <summary>
    /// Teardown method for Shell navigation.
    /// </summary>
    /// <param name="shell">The Shell instance to teardown.</param>
    public virtual void ShellTeardown(Shell shell)
    {
        if (NavigatedHandler is not null)
        {
            shell.Navigated -= NavigatedHandler;
        }
        
        Shell = null;
        NavigationParameter = null;
        IsReverseNavigation = false;
        NavigatedHandler = null;
    }

    /// <summary>
    /// Sets the navigation parameter to be passed during navigation.
    /// </summary>
    /// <param name="parameter">The navigation parameter to be assigned. Can be null.</param>
    public virtual void SetNavigationParameter(object? parameter)
    {
        NavigationParameter = parameter;
    }

    /// <summary>
    /// Retrieves the set of registered route keys in the navigation shell.
    /// </summary>
    /// <returns>A HashSet containing the registered route keys. If no route keys are found, returns an empty set.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the internal method for retrieving route keys cannot be accessed or invoked.</exception>
    private static HashSet<string> GetRegisteredRouteKeys()
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
    public virtual string RegisterRoute(Type pageType)
    {
        var prefixedRouteName = $"si_{pageType.Name}";
        var registeredRouteKeys = ShellInjectNavigation.GetRegisteredRouteKeys();
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
    /// Handles navigation events triggered by the Shell after navigation has occurred.
    /// Updates the view model with navigation data or invokes relevant lifecycle methods.
    /// </summary>
    /// <param name="sender">The object that raised the event, typically the Shell instance.</param>
    /// <param name="e">The event data containing details about the navigation that occurred.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task OnShellNavigatedAsync(object? sender, ShellNavigatedEventArgs e)
    {
        if ((Shell?.CurrentItem?.CurrentItem as IShellSectionController)?.PresentedPage is ContentPage
            {
                BindingContext: IShellInjectShellViewModel viewModel
            })
        {
            await viewModel.OnAppearedAsync();

            if (!viewModel.IsInitialized)
            {
                await viewModel.InitializedAsync();
                viewModel.IsInitialized = true;
            }
            
            if (NavigationParameter is not null)
            {
                if (IsReverseNavigation)
                {
                    await viewModel.ReverseDataReceivedAsync(NavigationParameter);
                }
                else
                {
                    await viewModel.DataReceivedAsync(NavigationParameter);
                }
            }
        }
    }

    /// <summary>
    /// Pushes a page onto the navigation stack.
    /// </summary>
    /// <param name="shell">The Shell instance to navigate on.</param>
    /// <param name="pageType">The type of the page to push.</param>
    /// <param name="tParameter">The parameter for the page.</param>
    /// <param name="animate">True to animate the transition, false otherwise. Default is true.</param>
    /// <returns>A Task representing the ongoing asynchronous operation.</returns>
    public virtual async Task PushAsync(Shell shell, Type pageType, object? tParameter, bool animate = true)
    {
        var route = RegisterRoute(pageType);
        ShellSetup(shell);
        SetNavigationParameter(tParameter);
        await shell.GoToAsync(route, animate);
        ShellTeardown(shell);
    }

    /// <summary>
    /// Resets the navigation and replaces the current main page
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="pageType"></param>
    /// <param name="tParameter"></param>
    /// <param name="animate"></param>
    /// <typeparam name="TParameter"></typeparam>
    public virtual async Task ReplaceAsync<TParameter>(Shell shell, Type? pageType, TParameter? tParameter, bool animate = true)
    {
        if (pageType == null)
        {
            return;
        }

        RegisterRoute(pageType);
        ShellSetup(shell, false);
        SetNavigationParameter(tParameter);
        await shell.Navigation.PopToRootAsync(false);
        await shell.GoToAsync($"//{pageType.Name}", animate: animate);
        await Task.Delay(300); // awaiting this so the page's binding context has time to set 
        ShellTeardown(shell);

        // If the Page wanting to replace is the same as the current page, then try triggering the DataReceivedAsync method
        // since Shell won't trigger the OnNavigating event in this scenario
        if (shell.CurrentPage?.GetType().Name == pageType.Name)
        {
            if (shell.CurrentPage is ContentPage { BindingContext: IShellInjectShellViewModel viewModel })
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
    public async Task ChangeTabAsync<TParameter>(Shell shell, int tabIndex, TParameter? tParameter, bool popToRootFirst)
    {
        ShellSetup(shell);

        var currentShellSection = shell.CurrentItem?.CurrentItem;
        if (currentShellSection == null || currentShellSection?.Items?.Count <= tabIndex)
        {
            ShellTeardown(shell);
            return; 
        }

        if (popToRootFirst && shell.Navigation?.NavigationStack?.Count > 1)
        {
            await shell.Navigation.PopToRootAsync(false);
        }
        
        if (currentShellSection != null)
        {
            currentShellSection.CurrentItem = currentShellSection.Items?[tabIndex];
        }
        
        await Task.Delay(50); // Wait briefly to let Shell update CurrentPage
        if (shell.CurrentPage?.BindingContext is IShellInjectShellViewModel vm)
        {
            await vm.DataReceivedAsync(tParameter);
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
    public async Task PopAsync<TResult>(Shell shell, TResult tResult, bool animate = true)
    {
        ShellSetup(shell);

        IsReverseNavigation = true;
        SetNavigationParameter(tResult);
        await shell.GoToAsync("..", animate);

        ShellTeardown(shell);
    }

    /// <summary>
    /// Closes the entire current Modal stack with Optional Parameter
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="data"></param>
    /// <param name="animate"></param>
    /// <returns></returns>
    public async Task PopModalStackAsync(Shell shell, object? data, bool animate)
    {
        var modalStack = shell.Navigation.ModalStack;
        var navStack = modalStack is { Count: > 0 } ? modalStack[^1].Navigation?.NavigationStack : null;
        if (navStack == null || navStack.Count == 0)
        {
            return;
        }

        // Loop through the navigation stack in reverse
        for (var index = navStack.Count - 1; index >= 0; index--)
        {
            if (index > 0)
            {
                await shell.GoToAsync("..", false); // Pop non modals
            }
            else
            {
                ShellSetup(shell);
                IsReverseNavigation = true;
                SetNavigationParameter(data);
                await shell.GoToAsync(".."); // Pop the modal and pass Data
                ShellTeardown(shell);
            }
        }
    }

    /// <summary>
    /// Navigates back to the specified page in the navigation stack.
    /// If the specified page is not found, navigates back to the root.
    /// </summary>
    /// <typeparam name="TResult">The type of the data to be passed to the target page or handled after navigation.</typeparam>
    /// <param name="shell">The Shell instance used for navigation.</param>
    /// <param name="pageType">The type of the page to navigate back to.</param>
    /// <param name="tResult">The result or data to be passed during navigation.</param>
    /// <exception cref="ArgumentNullException">Thrown if the shell or pageType is null.</exception>
    public async Task PopToAsync<TResult>(Shell shell, Type pageType, TResult tResult)
    {
        var navigationStack = shell.Navigation.NavigationStack;
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
                            IsReverseNavigation = true;
                            SetNavigationParameter(tResult);
                            animate = true;
                        }

                        await shell.GoToAsync("..", animate);
                        ShellTeardown(shell);
                    }

                    return;
                }
            }
        }

        // If pageType not found, return to root
        await shell.GoToAsync("//", true);
    }

    /// <summary>
    /// Pops all pages from the navigation stack and returns to the root page.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="shell">The Shell instance to perform the navigation.</param>
    /// <param name="tResult">The result parameter to be passed during navigation.</param>
    /// <param name="animate">True to animate the navigation, false otherwise. Default is true.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task PopToRootAsync<TResult>(Shell shell, TResult tResult, bool animate = true)
    {
        ShellSetup(shell);

        IsReverseNavigation = true;
        SetNavigationParameter(tResult);
        await shell.Navigation.PopToRootAsync(animate);

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
    public async Task PushMultiStackAsync<TParameter>(Shell shell, List<Type> pageTypes, TParameter tParameter, bool animate, bool animateAllPages)
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
                SetNavigationParameter(tParameter);
            }

            await shell.GoToAsync(route, type == lastState ? animate : animateAllPages);
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
    public async Task PushModalWithNavigation<TParameter>(Shell shell, ContentPage page, TParameter? tParameter, bool animate = true)
    {
        if (page == null)
        {
            throw new NullReferenceException(ShellInjectConstants.NullContentPageExceptionText);
        }
        
        ShellSetup(shell);

        await shell.Navigation.PushModalAsync(new NavigationPage(page), animate);
        if (page.BindingContext is IShellInjectShellViewModel vm)
        {
            if (tParameter != null)
            {
                await vm.DataReceivedAsync(tParameter);
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
    /// <exception cref="NullReferenceException"></exception>
    public async Task PushModalAsync(Shell shell, Type pageType, object? tParameter, bool animate = true)
    {
        if (Activator.CreateInstance(pageType) is ContentPage contentPage)
        {
            ShellSetup(shell);

             await shell.Navigation.PushModalAsync(contentPage, animate);
            if (contentPage.BindingContext is IShellInjectShellViewModel vm)
            {
                if (tParameter != null)
                {
                    await vm.DataReceivedAsync(tParameter);
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
    public async Task SendDataToPageAsync(Shell shell, Type? page, object? data = null)
    {
        if (page == null || shell.Navigation == null)
        {
            return;
        }
    
        var navigationStack = shell.Navigation.NavigationStack;
        if (navigationStack == null || navigationStack.Count == 0)
        {
            return;
        }
    
        var pageToSendDataTo = navigationStack
            .Where(p => p != null)
            .FirstOrDefault(p => p.GetType().Name == page.Name);
    
        if (pageToSendDataTo is { BindingContext: IShellInjectShellViewModel vm })
        {
            await vm.ReverseDataReceivedAsync(data);
        }
    }

    /// <summary>
    /// Shows/Creates a Popup of the Specified Type and passes in a data object
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="data"></param>
    /// <param name="onError"></param>
    /// <typeparam name="TPopup"></typeparam>
    /// <returns></returns>
    public async Task ShowPopupAsync<TPopup>(Shell shell, object? data, Action<Exception>? onError = null) where TPopup : Popup
    {
        if (shell.CurrentPage is null)
        {
            return;
        }

        if (Activator.CreateInstance(typeof(TPopup)) is not Popup popupPage)
        {
            return;
        }

        PopupStack.Add(popupPage);
        
        void OnPopupOpened(object? sender, EventArgs e)
        {
            popupPage.Opened -= OnPopupOpened;

            _ = Task.Run(async () =>
            {
                try
                {
                    if (popupPage.BindingContext is ShellInjectViewModel vm)
                    {
                        await vm.DataReceivedAsync(data);
                    }
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                }
            });
        }

        popupPage.Opened += OnPopupOpened;
        await shell.CurrentPage.ShowPopupAsync(popupPage);
    }

    /// <summary>
    /// Dismisses popup of the specified Type with Parameters
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="data"></param>
    /// <typeparam name="TPopup"></typeparam>
    public async Task DismissPopupAsync<TPopup>(Shell shell, object? data) where TPopup : Popup
    {
        var typedPopups = PopupStack?.OfType<TPopup>().ToList() ?? [];
        if (typedPopups.Count == 0)
        {
            return;
        }
        
        var latestPopup = typedPopups.Last();
        
        try
        {
            if (data is not null)
            {
                EventHandler? handler = null;
                handler = async (sender, args) =>
                {
                    latestPopup.Closed -= handler;
                        
                    if (shell.CurrentPage.BindingContext is ShellInjectViewModel vm)
                    {
                        try
                        {
                            await vm.ReverseDataReceivedAsync(data);
                        }
                        catch (Exception)
                        {
                            // just catch it
                        }
                    }
                };
                    
                latestPopup.Closed += handler;
            }
                
            await latestPopup.CloseAsync();
        }
        catch (ObjectDisposedException)
        {
            // Popup is already disposed, safe to ignore
        }
        finally
        {
            PopupStack?.Remove(latestPopup);
        }
    }
}