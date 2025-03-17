using System.Windows.Input;
using CommunityToolkit.Maui.Views;

namespace ShellInject.Extensions;

/// <summary>
/// Provides attached properties for extending the functionality of Shell pages in a Maui application.
/// </summary>
public static class ShellInjectPageExtensions
{
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
        }
        catch (Exception)
        {
            // ignored
        }
    }
    
    private static object ResolveViewModel(Type viewModelType)
    {
        if (Injector.ServiceProvider is not { } provider)
        {
            // If there's no service provider, just do a plain Activator create:
            return Activator.CreateInstance(viewModelType)
                   ?? throw new InvalidOperationException(
                       $"Unable to create instance of ViewModel. Type: {viewModelType.FullName}.");
        }
        
        // Check the services for any registered instances of the specified type
        var existingVm = provider.GetService(viewModelType);
        if (existingVm != null)
        {
            // Found a registered instance (singleton/scoped/transient).
            return existingVm;
        }

        // No existing service; create a new instance with DI injection support:
        return ActivatorUtilities.CreateInstance(provider, viewModelType)
               ?? throw new InvalidOperationException($"Unable to create instance of ViewModel. Type: {viewModelType.FullName}.");

    }

    /// <summary>
    /// Gets the ViewModel type of a bindable object.
    /// </summary>
    /// <param name="obj">The bindable object.</param>
    /// <returns>The ViewModel type.</returns>
    public static Type GetViewModelType(BindableObject obj)
    {
        return (Type)obj.GetValue(ViewModelTypeProperty);
    }
    
    public static void SetViewModelType(BindableObject obj, Type value)
    {
        obj.SetValue(ViewModelTypeProperty, value);
    }
    
    // Attached Properties for OnAppearing Events

    /// <summary>
    /// Provides an attached property for defining a command to be executed when a page appears.
    /// </summary>
    public static readonly BindableProperty OnAppearingCommandProperty =
        BindableProperty.CreateAttached(
            "OnAppearingCommand",
            typeof(ICommand),
            typeof(ShellInjectPageExtensions),
            null,
            propertyChanged: OnOnAppearingCommandChanged);

    /// <summary>
    /// Gets the attached command to be executed when a page appears.
    /// </summary>
    /// <param name="obj">The bindable object.</param>
    /// <returns>The command to be executed.</returns>
    public static ICommand GetOnAppearingCommand(BindableObject obj)
    {
        return (ICommand)obj.GetValue(OnAppearingCommandProperty);
    }

    /// <summary>
    /// Sets the OnAppearingCommand property on a bindable object.
    /// <param name="obj">The bindable object.</param>
    /// <param name="value">The ICommand to set as the OnAppearingCommand property.</param>
    /// </summary>
    public static void SetOnAppearingCommand(BindableObject obj, ICommand value)
    {
        obj.SetValue(OnAppearingCommandProperty, value);
    }

    /// <summary>
    /// Triggers when the value of the OnAppearingCommand attached property changes.
    /// </summary>
    /// <param name="bindable">The object on which the property is attached.</param>
    /// <param name="oldValue">The old value of the property.</param>
    /// <param name="newValue">The new value of the property.</param>
    private static void OnOnAppearingCommandChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ContentPage page) 
            return;
        
        if (oldValue is ICommand)
            page.Appearing -= OnPageAppearing;

        if (newValue is ICommand)
            page.Appearing += OnPageAppearing;
    }

    /// <summary>
    /// Executes a command when a page appears.
    /// </summary>
    /// <param name="sender">The object triggering the event.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnPageAppearing(object? sender, EventArgs e)
    {
        if (sender is not BindableObject bindable) 
            return;
        
        var command = GetOnAppearingCommand(bindable);
        command.Execute(null);
    }
}