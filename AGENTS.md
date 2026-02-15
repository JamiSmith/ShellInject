# AGENTS

## Repository overview
- ShellInject is a .NET MAUI library with a sample app and xUnit tests.
- Solution: `ShellInject.sln`.
- Library project: `ShellInject/ShellInject.csproj`.
- Sample app: `Sample/Sample.csproj`.
- Tests: `ShellInjectTests/ShellInjectTests.csproj`.

## Cursor/Copilot rules
- No `.cursor/rules/`, `.cursorrules`, or `.github/copilot-instructions.md` found.

## Prerequisites
- .NET SDK: tests target net10.0; library and sample target net10.0.
- MAUI workload: `dotnet workload install maui`.
- Platform toolchains required for iOS/Android builds (Xcode/Android SDK).
- No `global.json` is present; use a compatible SDK.

## Build commands
- Build the solution: `dotnet build ShellInject.sln -c Release`.
- Build the library only (net10.0): `dotnet build ShellInject/ShellInject.csproj -c Release -f net10.0`.
- Build the library for all MAUI TFMs: `dotnet build ShellInject/ShellInject.csproj -c Release`.
- Build the sample app (choose a target): `dotnet build Sample/Sample.csproj -c Debug -f net10.0-android`.
- Build tests: `dotnet build ShellInjectTests/ShellInjectTests.csproj -c Release`.
- Clean: `dotnet clean ShellInject.sln -c Release`.

## Test commands
- Run all tests: `dotnet test ShellInjectTests/ShellInjectTests.csproj -c Release`.
- Run tests without build: `dotnet test ShellInjectTests/ShellInjectTests.csproj -c Release --no-build`.
- Normal verbosity: `dotnet test ShellInjectTests/ShellInjectTests.csproj -c Release -v normal`.
- Coverage: `dotnet test ShellInjectTests/ShellInjectTests.csproj -c Release --collect:"XPlat Code Coverage"`.

## Single-test commands (xUnit)
- Fully qualified name:
  `dotnet test ShellInjectTests/ShellInjectTests.csproj -c Release --filter "FullyQualifiedName=ShellInjectTests.Navigation.PushNavigationTests.PushAsync_WhenCalled_ShouldInvokeSetNavigationParameterOnce"`
- Class name fragment:
  `dotnet test ShellInjectTests/ShellInjectTests.csproj -c Release --filter "FullyQualifiedName~ShellInjectTests.Navigation.ShellNavigatedTests"`
- Method name fragment:
  `dotnet test ShellInjectTests/ShellInjectTests.csproj -c Release --filter "FullyQualifiedName~OnShellNavigatedAsync_WhenShellNavigates"`
- Trait (if added later): `dotnet test ... --filter "Category=Navigation"`.

## Lint and formatting
- No repo-defined lint/format tooling found (.editorconfig/analyzers absent).
- Rely on compiler/nullable warnings from `dotnet build`.
- If formatters are added, prefer `dotnet format` with explicit config.

## C# layout and formatting
- File-scoped namespaces are standard (e.g., `namespace ShellInject;`).
- Indentation is 4 spaces; braces on new lines.
- Keep a blank line between `using` directives and namespace.
- `using` order: System first, third-party next, local last.
- One top-level type per file.
- Use XML doc comments for public API and non-obvious behavior.
- Keep line comments short and purposeful.
- Favor guard clauses and early returns over deep nesting.

## Naming conventions
- Types and methods: PascalCase.
- Parameters and locals: camelCase.
- Private fields: `_camelCase` (see `[ObservableProperty]` backing fields).
- Async methods end with `Async` unless event handlers.
- Test methods: `Method_WhenCondition_ShouldResult` with underscores.
- Interfaces use `I` prefix.
- Constants live in static classes and use PascalCase properties.

## Types and nullability
- Nullable reference types are enabled; annotate optional refs with `?`.
- Prefer pattern matching (`is not null`, `is Type t`) for checks.
- Navigation payloads use `object?` and `is` casts.
- Use `var` when the type is obvious from the RHS.
- Prefer `IReadOnlyList<T>`/`IEnumerable<T>` in signatures when possible.

## Async patterns
- Use `Task`/`Task<T>` with `async`/`await`.
- Return `Task.CompletedTask` for no-op overrides.
- Avoid `.Result`/`.Wait()` in UI code.
- Avoid fire-and-forget unless explicitly intended.

## Error handling
- Guard against null and invalid inputs early (return or throw).
- Configuration issues throw `InvalidOperationException`.
- Navigation helper failures sometimes throw `NullReferenceException`.
- Event handlers may catch and ignore errors to protect UI; keep scope small.
- Use `try/finally` when cleanup is required (handlers, popup stack).
- Prefer `ArgumentNullException` for public API parameters when appropriate.

## MVVM and MAUI patterns
- ViewModels typically inherit from `ShellInjectViewModel` or sample `BaseViewModel`.
- Use CommunityToolkit.Mvvm attributes (`[ObservableProperty]`, `[RelayCommand]`).
- Backing fields for `[ObservableProperty]` are `_camelCase`.
- Keep code-behind thin; bind ViewModels via `ShellInjectPageExtensions.ViewModelType`.
- Use `x:DataType` in XAML for compiled bindings when a ViewModel is known.
- Use `Shell` extension methods in `ShellInjectExtensions` for navigation.
- Navigation core lives in `ShellInject.Navigation.ShellInjectNavigation`.
- When adding navigation APIs, update `IShellInjectNavigation` and extensions.
- For back navigation data, use `PopAsync` + `ReverseDataReceivedAsync`.

## Dependency injection
- ShellInject relies on MAUI DI via `UseShellInject`.
- Prefer constructor injection for ViewModels/services.
- If a ViewModel is resolved by type, register it or provide a public ctor.
- Use `Injector.GetRequiredService<T>()` for required services.
- Use `Injector.GetService<T>()` only when missing is acceptable.

## XAML conventions
- Keep XML namespaces grouped and readable at the top of the file.
- Pages set `extensions:ShellInjectPageExtensions.ViewModelType`.
- Prefer `ContentPage`/`ContentView` with `x:Class` and minimal code-behind.
- Use `x:DataType` to enable compile-time binding checks.
- Keep layout markup simple and avoid logic in XAML code-behind.

## Constants and configuration
- User-facing error strings live in `ShellInject/Constants/ShellInjectConstants.cs`.
- Keep constants in a dedicated static class, not inline strings.
- `ShellInject.csproj` has `GeneratePackageOnBuild=true`; builds emit a NuGet package.
- Update `PackageReleaseNotes` for release changes.

## Documentation
- Root `README.md` is packaged as the NuGet readme; keep examples accurate.
- Update XML docs when changing public behavior.

## Tests
- Frameworks: xUnit + Moq; see `ShellInjectTests/Navigation`.
- Shared setup lives in `BaseNavigationTests`.
- Use `Record.ExceptionAsync` for async exception checks.
- Verify calls with `Times.Once`/`Times.Never`.
- Keep test data small and inline unless reused.

## Working in this repo
- Avoid editing generated assets in `Sample/Resources` unless needed.
- Keep public API changes documented.
- Platform-specific behavior uses `#if IOS` / `#if ANDROID`; keep blocks small.
- Prefer adding new API surface in `ShellInject/Extensions/`.

## Helpful entry points
- Navigation core: `ShellInject/Navigation/ShellInjectNavigation.cs`.
- Shell extension methods: `ShellInject/Extensions/ShellInjectExtensions.cs`.
- Page binding: `ShellInject/Extensions/ShellInjectPageExtensions.cs`.
- MAUI startup hook: `ShellInject/Extensions/ShellInjectMauiBuilderExtensions.cs`.
- ViewModel base: `ShellInject/ViewModels/ShellInjectViewModel.cs`.
- Service provider access: `ShellInject/Injector.cs`.
- Sample app setup: `Sample/MauiProgram.cs`.

## Notes for agents
- Use `-f net10.0` for fast library-only builds without MAUI platform deps.
- Tests target net10.0; ensure SDK supports that TFM.
- If analyzers or editor config are added, update this file.
- If new projects are added, update build/test command sections.
