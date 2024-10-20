namespace ShellInject;

/// <summary>
/// Contains extension methods for configuring ShellInject in a Maui app.
/// </summary>
public static class ShellInjectMauiBuilderExtensions
{
    /// <summary>
    /// Represents the service provider used for dependency injection in ShellInject framework.
    /// </summary>
    public static IServiceProvider? ServiceProvider { get; private set; }

    /// <summary>
    /// Uses ShellInject to configure the ShellInjectMauiBuilderExtensions in a Maui app.
    /// </summary>
    /// <param name="builder">The MauiAppBuilder instance.</param>
    /// <returns>The modified MauiAppBuilder instance.</returns>
    public static MauiAppBuilder UseShellInject(this MauiAppBuilder builder)
    {
        ServiceProvider = builder.Services.BuildServiceProvider();
        return builder;
    }
}