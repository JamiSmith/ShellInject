# ShellInject

ShellInject simplifies navigation and data transfer between ViewModels in .NET MAUI applications by leveraging the power of Shell Navigation combined with robust Dependency Injection. This package eliminates the need to manually register routes or ViewModels in your services, as ShellInject automatically handles route registration and ViewModel bindings for you.

With a single-line setup, ShellInject enhances maintainability and reduces boilerplate code, setting the BindingContext dynamically based on the ViewModelType property defined in XAML. This enables effortless parameter passing, lifecycle management, and seamless ViewModel interactions, making it ideal for applications with complex navigation flows or multi-page structures. 

## Setup

### MauiProgram.cs
To enable ShellInject and integrate its features into your app, add the `.UseShellInject()` line of code in your `MauiProgram.cs`:
```
 var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .RegisterServices()
        .UseShellInject();

 return builder.Build();
```

## Usage

### ViewModels
Ensure your ViewModels inherit from `ShellInjectViewModel` to handle navigation events and parameter passing. You can override the following methods based on your needs:

- `OnAppearedAsync()` – Triggered everytime a page is navigated to.
- `InitializedAsync()` - Invoked once when the viewmodel is loaded for the first time.
- `DataReceivedAsync(object? parameter)` – Triggered when data is passed to the ViewModel (forward navigation).
- `ReverseDataReceivedAsync(object? parameter)` – Handles data returned when navigating back from a page (back navigation).

ShellInject now handles the following methods out of the box. Just override them in your viewmodel.

- `OnAppearing()` – Called when the page is about to appear.
- `OnDisappearing()` – Called when the page is about to disappear.

## ContentPage

In your `ContentPage.xaml` file, specify the ViewModel to bind by setting the `ViewModelType` property. This ensures that the correct ViewModel is injected and bound to the page:

```
xmlns:ez="clr-namespace:ShellInject;assembly=ShellInject"
ez:ShellInjectPageExtensions.ViewModelType="{x:Type vm:MainViewModel}"
```


## Navigation

ShellInject provides a static navigation API via `ShellNavigation` (uses `Shell.Current` by default). Existing `Shell` extension methods remain but are deprecated and forward to the static API.

**Example:**
```csharp
await ShellNavigation.PushAsync<DetailsPage>(parameter: new { id = 123, name = "John" });
```

Available navigation methods:

***Pushing:***
```
ShellNavigation.PushAsync<TPageType>(parameter)
ShellNavigation.PushMultiStackAsync(pageTypes, parameter)
```

***Modals:***
```
ShellNavigation.PushModalWithNavigationAsync(page, parameter)
ShellNavigation.PushModalAsync<TPageType>(parameter)
```

***Popping:***
```
ShellNavigation.PopAsync(parameter)
ShellNavigation.PopModalStackAsync(data)
ShellNavigation.PopToAsync<TPageType>(parameter)
ShellNavigation.PopToRootAsync(parameter)
```

***Popups:***

```
ShellNavigation.ShowPopupAsync<TPopup>(data)
ShellNavigation.DismissPopupAsync<TPopup>(data)
```

***Replacing Root Pages in a Flyout Menu:***
```
ShellNavigation.ReplaceAsync<TPageType>(parameter)
```

***Changing Tabs:***
```
ShellNavigation.ChangeTabAsync(tabIndex, parameter)
```

## Helper methods:

This method looks for the specified Page on the stack and sends the data using the ReverseDataReceivedAsync method.
```
ShellNavigation.SendDataToPageAsync<TPageType>(data)
```
Note: the `Shell` extension method is deprecated. Use the static API instead.

## ServiceProvider
Retrieving services manually, you can use the following:
```
var requiredService = Injector.GetRequiredService<ISampleService>();
var optionalService = Injector.GetService<ISampleService>();
```
Note: `GetService` returns null if the service is not registered.
