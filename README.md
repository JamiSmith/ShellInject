# ShellInject

ShellInject simplifies navigation and data transfer between ViewModels in .NET MAUI applications by leveraging the power of Shell Navigation combined with robust Dependency Injection. This package eliminates the need to manually register routes or ViewModels in your services, as ShellInject automatically handles route registration and ViewModel bindings for you.

With a single-line setup, ShellInject enhances maintainability and reduces boilerplate code, setting the BindingContext dynamically based on the ViewModelType property defined in XAML. This enables effortless parameter passing, lifecycle management, and seamless ViewModel interactions, making it ideal for applications with complex navigation flows or multi-page structures. 

## Setup

### MauiProgram.cs
To enable ShellInject and integrate its features into your app, add the `.UseShellInject()` line of code in your `MauiProgram.cs` file after registering your app’s services and interfaces:
```
 var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .RegisterServices()
        .UseShellInject();

 return builder.Build();
```

Note: For now, it is recommended to add the `.UseShellInject()` after all your services have been registered.

## Usage

### ViewModels
Ensure your ViewModels inherit from `ShellInjectViewModel` to handle navigation events and parameter passing. You can override the following methods based on your needs:

- `DataReceivedAsync(object? parameter)` – Triggered when data is passed to the ViewModel.
- `ReverseDataReceivedAsync(object? parameter)` – Handles data returned when navigating back from a page.


These two methods require the xaml markup shown below in order for them to trigger.
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
await Shell.Current.PushAsync<DetailsPage>(new { id = 123, name = "John" });
```

Available navigation methods:

***Pushing:***
```
PushAsync<TPageType>(someData)
PushMultiStackAsync(pageTypes, someData)
```

***Modals:***
```
PushModalWithNavigationAsync(contentPage, someData)
PushModalAsync<TPageType>(someData)
```

***Popping:***
```
PopAsync(someData)
PopModalStackAsync(someData)
PopToAsync<TPageType>(someData)
PopToRootAsync(someData)
```

***Popups:***

```
ShowPopupAsync<TPopup>(someData)
DismissPopupAsync<TPopup>(someData)
```

***Replacing Root Pages in a Flyout Menu:***
```
ReplaceAsync<TPageType>(someData)
```

***Changing Tabs:***
```
ChangeTabAsync(tabIndex, someData)
```

## Helper methods:

This method looks for the specified Page on the stack and sends the data using the ReverseDataReceivedAsync method.
```
SendDataToPageAsync<TPageType>(someData)
```


***TODO List:***

1. Update Sample Project
2. Add Unit Tests
3. Setup CI/CD

