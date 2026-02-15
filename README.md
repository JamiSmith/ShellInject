# ShellInject

ShellInject is a .NET MAUI library that makes Shell navigation feel effortless. It provides a clean, typed navigation API, wires ViewModels from XAML, and automatically delivers navigation data and lifecycle events. With a single startup line and a single attached property in XAML, your pages are ready to navigate, receive parameters, and stay lifecycle-aware without manual route registration or boilerplate setup.

Why use it? ShellInject keeps your ViewModels focused on behavior, not plumbing. It reduces navigation friction, enables consistent data passing in both directions, and scales cleanly as your app grows in complexity.

## Highlights

- Static `ShellNavigation` API that uses `Shell.Current` by default (or an explicit `Shell` if you prefer).
- Automatic route registration for pages, no manual route strings.
- ViewModel binding via a single attached property in XAML.
- Reliable lifecycle hooks: appear, disappear, first-time initialization, and data delivery.
- Built-in helpers for modals, tabs, popups, and back-navigation data.

## Requirements

- .NET MAUI Shell-based app.
- If you use popups, install and register `CommunityToolkit.Maui` in your app.

## Quick Start

### 1) Enable ShellInject

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseShellInject();

return builder.Build();
```

### 2) Bind a ViewModel in XAML

```xml
xmlns:extensions="clr-namespace:ShellInject.Extensions;assembly=ShellInject"
xmlns:vm="clr-namespace:YourApp.ViewModels"

extensions:ShellInjectPageExtensions.ViewModelType="{x:Type vm:MainViewModel}"
x:DataType="vm:MainViewModel"
```

ShellInject will resolve the ViewModel from DI if registered, otherwise it will create one and inject dependencies via `ActivatorUtilities`.

### 3) Inherit from ShellInjectViewModel

```csharp
public class MainViewModel : ShellInjectViewModel
{
    public override Task InitializedAsync()
    {
        // First-time setup
        return Task.CompletedTask;
    }

    public override Task DataReceivedAsync(object? parameter)
    {
        // Forward navigation data
        return Task.CompletedTask;
    }

    public override Task ReverseDataReceivedAsync(object? parameter)
    {
        // Back navigation data
        return Task.CompletedTask;
    }
}
```

### 4) Navigate with ShellNavigation

```csharp
await ShellNavigation.PushAsync<DetailsPage>(parameter: new { id = 123 });
```

## ViewModel Lifecycle and Data Flow

ShellInject wires lifecycle and data delivery automatically for ViewModels that implement `IShellInjectShellViewModel` (already implemented by `ShellInjectViewModel`).

- `OnAppearing()` and `OnDisAppearing()` are invoked on page appear/disappear.
- `OnAppearedAsync()` runs after navigation completes.
- `InitializedAsync()` runs once per ViewModel instance (first time it appears).
- `DataReceivedAsync(object? parameter)` runs on forward navigation with a parameter.
- `ReverseDataReceivedAsync(object? parameter)` runs when data is returned on back navigation.

## Navigation API

`ShellNavigation` is the recommended entry point. It uses `Shell.Current` by default and throws a clear exception if a Shell cannot be found. If you use multiple windows, pass an explicit `Shell` instance.

```csharp
await ShellNavigation.PushAsync<DetailsPage>(shell: myShell, parameter: data);
```

### Pushing

```csharp
ShellNavigation.PushAsync<TPageType>(parameter, animate);
ShellNavigation.PushMultiStackAsync(pageTypes, parameter, animate, animateAllPages);
```

### Modals

```csharp
ShellNavigation.PushModalWithNavigationAsync(page, parameter, animate);
ShellNavigation.PushModalAsync<TPageType>(parameter, animate);
```

### Popping

```csharp
ShellNavigation.PopAsync(parameter, animate);
ShellNavigation.PopModalStackAsync(data, animate);
ShellNavigation.PopToAsync<TPageType>(parameter);
ShellNavigation.PopToRootAsync(parameter, animate);
```

### Popups

```csharp
ShellNavigation.ShowPopupAsync<TPopup>(data, onError);
ShellNavigation.DismissPopupAsync<TPopup>(data);
```

### Replace Root (Flyout)

```csharp
ShellNavigation.ReplaceAsync<TPageType>(parameter, animate);
```

### Tabs

```csharp
ShellNavigation.ChangeTabAsync(tabIndex, parameter, popToRootFirst);
```

### Helper

```csharp
ShellNavigation.SendDataToPageAsync<TPageType>(data);
```

## Data Return Example

```csharp
// Details page
await ShellNavigation.PopAsync(parameter: "Saved!");

// Main view model
public override Task ReverseDataReceivedAsync(object? parameter)
{
    var message = parameter as string;
    return Task.CompletedTask;
}
```

## Dependency Injection Helpers

```csharp
var requiredService = Injector.GetRequiredService<ISampleService>();
var optionalService = Injector.GetService<ISampleService>();
```

Note: `GetService` returns null if the service is not registered.

## Deprecated API

The old `Shell` extension methods are still available for backwards compatibility but are marked `[Obsolete]`. Use `ShellNavigation` for new code.

## Troubleshooting

- `InvalidOperationException` about Shell not found means `Shell.Current` is unavailable. Ensure your app is using a `Shell` as the root page, or pass an explicit `Shell` instance to `ShellNavigation`.
- `PushMultiStackAsync` requires at least one page type in the list.

## Sample

See the `Sample` project in this repository for end-to-end usage patterns.
