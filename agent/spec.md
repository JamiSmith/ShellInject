# ShellInject agent spec

## Project context
ShellInject is a .NET MAUI NuGet package that simplifies Shell navigation and data transfer between ViewModels. It registers routes, binds ViewModels via XAML, and triggers lifecycle events for ViewModels (appearing, disappearing, and async lifecycle hooks).

## Goals for this work
1) Deprecate the Shell extension methods and introduce a new, static navigation API.
2) Improve lifecycle event reliability on app startup, Shell replacement, and Replace navigation scenarios.
3) Review and improve related classes for clarity, robustness, and consistency.
4) Update tests, sample app, and README to reflect the new API and behavior.

## Requirements
- Add a static navigation entry point (tentative name: ShellNavigation) that exposes all navigation methods.
- Existing Shell extension methods remain but are marked [Obsolete] and route to the new API.
- Navigation API remains easy to use (no extra setup required for typical consumers).
- Lifecycle events should reliably trigger for initial startup and Shell replacement scenarios.
- Update tests to cover new API and lifecycle behavior; remove outdated tests.
- Update Sample app to demonstrate the new navigation API and existing features.
- Update README with the new API usage and deprecation notes.

## Constraints
- Keep public API changes backward compatible where possible.
- Follow repo style and patterns in AGENTS.md.
- Use nullable reference types and guard clauses as appropriate.
- Keep changes focused and avoid unnecessary refactors.

## Notes
- New API should be static. If DI is used internally, hide it behind the static API.
- Avoid breaking existing users; use Obsolete attributes instead of removal.
