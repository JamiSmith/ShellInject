using CommunityToolkit.Maui.Views;
using ShellInject.Navigation;

namespace ShellInject;

/// <summary>
/// Provides extension methods for the Shell class to simplify navigation operations.
/// </summary>
public static class ShellInjectExtensions
{
    /// <summary>
    /// Pushes a ContentPage onto the navigation stack.
    /// </summary>
    /// <param name="shell">The Shell instance.</param>
    /// <param name="pageType"></param>
    /// <param name="parameter">An optional parameter to pass to the pushed page.</param>
    /// <param name="animate">A boolean value indicating whether to animate the push operation. Default is true.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    [Obsolete("Use PushAsync<TPageType> instead.")]
    public static Task PushAsync(this Shell shell, Type pageType, object? parameter = null, bool animate = true)
    {
        return ShellInjectNavigation.Instance.PushAsync(shell, pageType, parameter, animate);
    }
    
    public static Task PushAsync<TPageType>(this Shell shell, object? parameter = null, bool animate = true) where TPageType : ContentPage
    {
        return ShellInjectNavigation.Instance.PushAsync(shell, typeof(TPageType), parameter, animate);
    }
    
    /// <summary>
    /// Resets the navigation and replaces the current main page.
    /// Helpful for Flyout Menus
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="pageType"></param>
    /// <param name="parameter"></param>
    /// <param name="animate"></param>
    /// <returns></returns>
    [Obsolete("Use ReplaceAsync<TPageType> instead.")]
    public static Task ReplaceAsync(this Shell shell, Type? pageType, object? parameter = null, bool animate = true)
    {
        return ShellInjectNavigation.Instance.ReplaceAsync(shell, pageType, parameter, animate);
    }
    
    public static Task ReplaceAsync<TPageType>(this Shell shell, object? parameter = null, bool animate = true) where TPageType : ContentPage
    {
        return ShellInjectNavigation.Instance.ReplaceAsync(shell, typeof(TPageType), parameter, animate);
    }

    /// <summary>
    /// Pops the current page from the navigation stack, returning to the previous page.
    /// </summary>
    /// <param name="shell">The Shell instance.</param>
    /// <param name="parameter">The optional parameter to pass to the previous page.</param>
    /// <param name="animate">Whether to animate the navigation transition. The default is true.</param>
    /// <returns>A task that represents the asynchronous operation of popping the page. The task completes when the navigation is finished.</returns>
    public static Task PopAsync(this Shell shell, object? parameter = null, bool animate = true)
    {
        return ShellInjectNavigation.Instance.PopAsync(shell, parameter, animate);
    }
    
    /// <summary>
    /// Pops the navigation stack back to the specified page type
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="pageType"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [Obsolete("Use PopToAsync<TPageType> instead.")]
    public static Task PopToAsync(this Shell shell, Type pageType, object? parameter = null)
    {
        return ShellInjectNavigation.Instance.PopToAsync(shell, pageType, parameter);
    }
    
    public static Task PopToAsync<TPageType>(this Shell shell, object? parameter = null)
    {
        return ShellInjectNavigation.Instance.PopToAsync(shell, typeof(TPageType), parameter);
    }

    /// <summary>
    /// Pops all pages from the navigation stack and returns to the root page.
    /// </summary>
    /// <param name="shell">The Shell instance to perform the navigation.</param>
    /// <param name="parameter"></param>
    /// <param name="animate">True to animate the navigation, false otherwise. Default is true.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static Task PopToRootAsync(this Shell shell, object? parameter = null, bool animate = true)
    {
        return ShellInjectNavigation.Instance.PopToRootAsync(shell, parameter, animate);
    }

    /// <summary>
    /// Changes the active tab of the Shell and navigates to the specified tab index asynchronously.
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="tabIndex">The index of the tab to navigate to.</param>
    /// <param name="parameter">An optional parameter to pass to the tab navigation.</param>
    /// <param name="popToRootFirst">A flag indicating whether to pop to the root of the navigation stack before changing the tab.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task ChangeTabAsync(this Shell shell, int tabIndex, object? parameter = null, bool popToRootFirst = true)
    {
        return ShellInjectNavigation.Instance.ChangeTabAsync(shell, tabIndex, parameter, popToRootFirst);
    }
    
    /// <summary>
    /// Pushes multiple navigation stacks onto the Shell using a list of page types.
    /// </summary>
    /// <param name="shell">The Shell instance.</param>
    /// <param name="pageTypes">A list of page types to push onto the Shell.</param>
    /// <param name="parameter">The optional parameter to pass to the pages.</param>
    /// <param name="animate">A boolean value indicating whether to animate the page transitions. Default is true.</param>
    /// <param name="animateAllPages">A boolean value indicating whether to animate all pages in the navigation stack. Default is false.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static Task PushMultiStackAsync(this Shell shell, List<Type> pageTypes, object? parameter = null, bool animate = true, bool animateAllPages = false)
    {
        return ShellInjectNavigation.Instance.PushMultiStackAsync(shell, pageTypes, parameter, animate, animateAllPages);
    }
    
    /// <summary>
    /// Pushes a modal page onto the navigation stack.
    /// </summary>
    /// <param name="shell">The Shell instance.</param>
    /// <param name="page">The modal page to push.</param>
    /// <param name="parameter">An optional parameter to pass to the page.</param>
    /// <param name="animate">A flag indicating whether to animate the transition. The default value is true.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static Task PushModalWithNavigationAsync(this Shell shell, ContentPage page, object? parameter = null, bool animate = true)
    {
        return ShellInjectNavigation.Instance.PushModalWithNavigation(shell, page, parameter, animate);
    }

    /// <summary>
    /// Pushes a modal page
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="pageType"></param>
    /// <param name="tParameter"></param>
    /// <param name="animate"></param>
    /// <returns></returns>
    [Obsolete("Use PushModalAsync<TPageType> instead.")]
    public static Task PushModalAsync(this Shell shell, Type pageType, object? tParameter = null, bool animate = true)
    {
        return ShellInjectNavigation.Instance.PushModalAsync(shell, pageType, tParameter, animate);
    }
    
    public static Task PushModalAsync<TPageType>(this Shell shell, object? tParameter = null, bool animate = true)
    {
        return ShellInjectNavigation.Instance.PushModalAsync(shell, typeof(TPageType), tParameter, animate);
    }

    /// <summary>
    /// Looks for the specified Page on the stack and sends the data using the ReverseDataReceivedAsync method
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="page"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [Obsolete("Use SendDataToPageAsync<TPageType> instead.")]
    public static Task SendDataToPageAsync(this Shell shell, Type? page, object? data = null)
    {
        return ShellInjectNavigation.Instance.SendDataToPageAsync(shell, page, data);
    }
    
    public static Task SendDataToPageAsync<TPageType>(this Shell shell, object? data = null)
    {
        return ShellInjectNavigation.Instance.SendDataToPageAsync(shell, typeof(TPageType), data);
    }

    /// <summary>
    /// Shows/Creates a Popup of the Specified Type and passes in a data object
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="data"></param>
    /// <typeparam name="TPopup"></typeparam>
    /// <returns></returns>
    public static Task ShowPopupAsync<TPopup>(this Shell shell, object? data = null)
    {
        return ShellInjectNavigation.Instance.ShowPopupAsync<TPopup>(shell, data);
    }

    /// <summary>
    /// Dismisses all popups of the specified Type
    /// </summary>
    /// <param name="shell"></param>
    /// <typeparam name="TPopup"></typeparam>
    /// <returns></returns>
    public static Task DismissPopupAsync<TPopup>(this Shell shell) where TPopup : Popup
    {
        return ShellInjectNavigation.Instance.DismissPopupAsync<TPopup>(shell);
    }
}