# Implementation plan

## Order of work
1) Create the new static navigation entry point and route existing extensions to it.
2) Review and improve ShellInjectViewModel for clarity and reliability.
3) Review and improve ShellInjectPageExtensions for lifecycle hookup safety and DI usage.
4) Review and improve Injector for clarity and error messaging consistency.
5) Fix lifecycle handling in UseShellInject for startup and Shell replacement scenarios.
6) Update tests to align with new API and lifecycle behavior.
7) Clean up unused or obsolete code.
8) Update Sample app to use the new navigation API.
9) Update README with new usage and deprecations.

## Acceptance criteria
- New static navigation API exists and matches prior functionality.
- Extension methods are [Obsolete] and forward to the new API.
- Lifecycle events trigger reliably on startup and Shell replacement cases.
- Tests cover new API and lifecycle behavior and pass.
- Sample app demonstrates new usage for navigation and data passing.
- README documents the new API and notes deprecated extensions.

## Commit strategy
- Commit after each completed task in agent/tasks.md.
- Keep commits scoped to a single task.
- Use concise commit messages focused on the why.

## Assumptions
- ShellNavigation can use Shell.Current by default and throw a clear exception if unavailable.
- Obsolete attributes should be non-breaking (warning only).
