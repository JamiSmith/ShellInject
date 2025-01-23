# ShellInject

ShellInject enhances the data transfer experience between ViewModels in .NET MAUI applications by leveraging Shell Navigation’s powerful features combined with robust Dependency Injection. By simplifying parameter passing, lifecycle management, and ViewModel interactions, ShellInject allows for more efficient and maintainable navigation, especially in applications with complex data flows or multi-page navigation structures.
## Setup

### MauiProgram.cs
To enable ShellInject and integrate its features into your app, add the following line of code in your `MauiProgram.cs` file after registering your app’s services and interfaces:
```
 var builder = MauiApp.CreateBuilder();
 builder.UseShellInject();
```

## Usage

### ViewModels
Ensure your ViewModels inherit from `ShellInjectViewModel` to handle navigation events and parameter passing. You can override the following methods based on your needs:

- `DataReceivedAsync(object? parameter)` – Triggered when data is passed to the ViewModel.
- `ReverseDataReceivedAsync(object? parameter)` – Handles data returned when navigating back from a page.
- `OnAppearing()` – Called when the page is about to appear.
- `OnDisappearing()` – Called when the page is about to disappear.

## ContentPage

In your `ContentPage.xaml` file, specify the ViewModel to bind by setting the `ViewModelType` property. This ensures that the correct ViewModel is injected and bound to the page:

```
xmlns:ez="clr-namespace:ShellInject;assembly=ShellInject"
ez:ShellInjectPageExtensions.ViewModelType="{x:Type vm:MainViewModel}"
```

Optionally you can set up listeners for OnAppearing and OnDisappearing events

```
ez:ShellInjectPageExtensions.OnAppearingCommand="{Binding OnAppearingCommand}"
ez:ShellInjectPageExtensions.OnDisappearingCommand="{Binding OnDisAppearingCommand}"
```


## Navigation

ShellInject provides extension methods to simplify navigation between pages and passing parameters between ViewModels:

**Example:**
```csharp
await Shell.Current.PushAsync(typeof(DetailsPage), new { id = 123, name = "John" });
```

Available navigation methods:

```
Task PushAsync(this Shell shell, Type pageType, object? parameter, bool animate = true)
Task PopAsync(this Shell shell, object? parameter = null, bool animate = true)
Task PopToAsync(this Shell shell, Type pageType, object? parameter = null, bool animate = true)
Task PopToRootAsync(this Shell shell, object? parameter = null, bool animate = true)
Task ChangeTabAsync(this Shell shell, int tabIndex, object? parameter = null, bool popToRootFirst = true)
Task PushMultiStackAsync(this Shell shell, List<Type> pageTypes, object? parameter = null, bool animate = true, bool animateAllPages = false)
Task PushModalAsync(this Shell shell, ContentPage page, object? parameter = null, bool animate = true)
Task ReplaceAsync(this Shell shell, Type? pageType, object? parameter = null, bool animate = true)
```

## Helper methods:

This method looks for the specified Page on the stack and sends the data using the ReverseDataReceivedAsync method.
```
Task SendDataToPageAsync(this Shell shell, Type? page, object data)
```


***TODO List:***

1. Update Sample Project
2. Add Unit Tests
3. Setup CI/CD
4. Add Support for Popups

