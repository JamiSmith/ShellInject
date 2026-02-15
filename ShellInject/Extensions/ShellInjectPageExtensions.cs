using CommunityToolkit.Maui.Views;
using ShellInject.Interfaces;

namespace ShellInject.Extensions;

/// <summary>
/// Provides attached properties for extending the functionality of Shell pages in a Maui application.
/// </summary>
public static class ShellInjectPageExtensions
{
    // A private helper class to store event handlers so they can be detached later.
    private class PageLifecycleToken
    {
        public EventHandler AppearingHandler { get; }
        public EventHandler DisappearingHandler { get; }

        public PageLifecycleToken(EventHandler appearing, EventHandler disappearing)
        {
            AppearingHandler = appearing;
            DisappearingHandler = disappearing;
        }
    }

    // An attached property to hold the lifecycle token object on each page.
    private static readonly BindableProperty PageLifecycleTokenProperty =
        BindableProperty.CreateAttached(
            "PageLifecycleToken",
            typeof(PageLifecycleToken),
            typeof(ShellInjectPageExtensions),
            null);

    private static void DetachPageLifecycleHandlers(ContentPage page)
    {
        if (page.GetValue(PageLifecycleTokenProperty) is not PageLifecycleToken token)
        {
            return;
        }

        page.Appearing -= token.AppearingHandler;
        page.Disappearing -= token.DisappearingHandler;
        page.SetValue(PageLifecycleTokenProperty, null);
    }

    
    // Attached Properties for ViewModel Type

    /// <summary>
    /// Provides attached properties for extending the functionality of Shell pages in a Maui application.
    /// </summary>
    public static readonly BindableProperty ViewModelTypeProperty =
        BindableProperty.CreateAttached(
            "ViewModelType",
            typeof(Type),
            typeof(ShellInjectPageExtensions),
            null,
            propertyChanged: OnViewModelTypePropertyChanged);

    /// <summary>
    /// Called when the value of the ViewModelType attached property changes.
    /// </summary>
    /// <param name="bindable">The bindable object on which the property is attached.</param>
    /// <param name="oldValue">The old value of the property.</param>
    /// <param name="newValue">The new value of the property.</param>
    private static void OnViewModelTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var bindablePage = bindable as ContentPage;
        if (bindablePage is not null)
        {
            DetachPageLifecycleHandlers(bindablePage);
        }

        if (newValue is not Type viewModelType)
        {
            return;
        }
        
        try
        {
            var viewModelInstance = ResolveViewModel(viewModelType);
            switch (bindable)
            {
                case ContentPage page:
                    page.BindingContext = viewModelInstance;
                    break;
                case Popup popup:
                    popup.BindingContext = viewModelInstance;
                    break;
                case ContentView contentView:
                    contentView.BindingContext = viewModelInstance;
                    break;
                case Shell shell:
                    shell.BindingContext = viewModelInstance;
                    break;
                case FlyoutPage flyoutPage:
                    flyoutPage.BindingContext = viewModelInstance;
                    break;
                case NavigationPage navigationPage:
                    navigationPage.BindingContext = viewModelInstance;
                    break;
                case TabbedPage tabbedPage:
                    tabbedPage.BindingContext = viewModelInstance;
                    break;
                case CollectionView collectionView:
                    collectionView.BindingContext = viewModelInstance;
                    break;
                case Layout layout: // Covers Frame, Grid, StackLayout, and other layouts
                    layout.BindingContext = viewModelInstance;
                    break;
            }
            
            if (bindablePage is null)
            {
                return;
            }

            // If a new ViewModelType is being set, attach new handlers
            if (bindablePage.BindingContext is not IShellInjectShellViewModel vmInstance)
            {
                return;
            }
            
            // Attach new event handlers
            EventHandler appearingHandler = (s, e) =>
            {
                try
                {
                    vmInstance.OnAppearing();
                }
                catch
                {
                    // ignored
                }
            };
            EventHandler disappearingHandler = (s, e) =>
            {
                try
                {
                    vmInstance.OnDisAppearing();
                }
                catch
                {
                    // ignored
                }
            };

            // Register them on the page
            bindablePage.Appearing += appearingHandler;
            bindablePage.Disappearing += disappearingHandler;

            // Store them in the attached property so we can unregister later
            var newToken = new PageLifecycleToken(appearingHandler, disappearingHandler);
            bindablePage.SetValue(PageLifecycleTokenProperty, newToken);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    /// <summary>
    /// Resolves and creates an instance of the specified ViewModel type.
    /// </summary>
    /// <param name="viewModelType">The type of the ViewModel to be resolved.</param>
    /// <returns>
    /// An object instance of the specified ViewModel type. May return an existing instance
    /// if registered in the dependency injection container or create a new one.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the ViewModel instance cannot be created either because the type is invalid
    /// or the ServiceProvider is not properly configured.
    /// </exception>
    private static object ResolveViewModel(Type viewModelType)
    {
        if (Injector.ServiceProvider is not { } provider)
        {
            // If there's no service provider, just do a plain Activator create:
            return Activator.CreateInstance(viewModelType)
                   ?? throw new InvalidOperationException(
                       $"Unable to create instance of ViewModel. Type: {viewModelType.FullName}.");
        }
        
        return ActivatorUtilities.GetServiceOrCreateInstance(provider, viewModelType)
               ?? throw new InvalidOperationException($"Unable to create instance of ViewModel. Type: {viewModelType.FullName}.");

    }

    /// <summary>
    /// Retrieves the value of the ViewModelType attached property from the specified bindable object.
    /// </summary>
    /// <param name="obj">The bindable object from which to retrieve the ViewModelType.</param>
    /// <returns>The type of the ViewModel set on the bindable object.</returns>
    public static Type GetViewModelType(BindableObject obj)
    {
        return (Type)obj.GetValue(ViewModelTypeProperty);
    }

    /// <summary>
    /// Sets the value of the ViewModelType attached property for the specified bindable object.
    /// </summary>
    /// <param name="obj">The bindable object on which the ViewModelType property is being set.</param>
    /// <param name="value">The Type value to be set for the ViewModelType property.</param>
    public static void SetViewModelType(BindableObject obj, Type value)
    {
        obj.SetValue(ViewModelTypeProperty, value);
    }
}
