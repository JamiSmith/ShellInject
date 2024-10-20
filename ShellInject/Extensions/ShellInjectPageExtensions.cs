using System.Windows.Input;

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
            default(Type),
            propertyChanged: OnViewModelTypePropertyChanged);

    /// <summary>
    /// Called when the value of the ViewModelType attached property changes.
    /// </summary>
    /// <param name="bindable">The bindable object on which the property is attached.</param>
    /// <param name="oldValue">The old value of the property.</param>
    /// <param name="newValue">The new value of the property.</param>
    private static void OnViewModelTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ContentPage page || newValue is not Type viewModelType)
            return;
        
        try
        {
            var viewModelInstance = ShellInjectMauiBuilderExtensions.ServiceProvider == null
                ? Activator.CreateInstance(viewModelType)
                : ActivatorUtilities.CreateInstance(ShellInjectMauiBuilderExtensions.ServiceProvider, viewModelType);

            page.BindingContext = viewModelInstance;
        }
        catch (Exception)
        {
            // Just catch it
        }
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
    
    internal static void SetViewModelType(BindableObject obj, Type value)
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
    internal static void SetOnAppearingCommand(BindableObject obj, ICommand value)
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
    
    // Attached properties for OnDisappearing events

    /// <summary>
    /// Provides an attached property for defining a command to be executed when a page disappears.
    /// </summary>
    public static readonly BindableProperty OnDisappearingCommandProperty =
        BindableProperty.CreateAttached(
            "OnDisappearingCommand",
            typeof(ICommand),
            typeof(ShellInjectPageExtensions),
            null,
            propertyChanged: OnOnDisappearingCommandChanged);

    internal static ICommand GetOnDisappearingCommand(BindableObject obj)
    {
        return (ICommand)obj.GetValue(OnDisappearingCommandProperty);
    }

    /// <summary>
    /// Provides an attached property for defining a command to be executed when a page disappears.
    /// </summary>
    /// <param name="obj">The bindable object to which the attached property is attached.</param>
    /// <param name="value">The command to be executed when the page disappears.</param>
    public static void SetOnDisappearingCommand(BindableObject obj, ICommand value)
    {
        obj.SetValue(OnDisappearingCommandProperty, value);
    }

    /// <summary>
    /// Event handler for the OnDisappearingCommand property. Executes the command when the page disappears.
    /// </summary>
    /// <param name="bindable">The bindable object.</param>
    /// <param name="oldValue">The old value of the property.</param>
    /// <param name="newValue">The new value of the property.</param>
    private static void OnOnDisappearingCommandChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ContentPage page) 
            return;
        
        if (oldValue is ICommand)
            page.Disappearing -= OnPageDisappearing;

        if (newValue is ICommand)
            page.Disappearing += OnPageDisappearing;
    }

    /// <summary>
    /// Represents an attached property for defining a command to be executed when a page disappears.
    /// </summary>
    /// <remarks>
    /// The attached property must be set on a ContentPage instance. The specified command will be executed when the page disappears.
    /// </remarks>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnPageDisappearing(object? sender, EventArgs e)
    {
        if (sender is not BindableObject bindable) 
            return;
        
        var command = GetOnDisappearingCommand(bindable);
        command.Execute(null);
    }
}