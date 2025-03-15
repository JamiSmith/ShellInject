namespace ShellInject.Services;

/// <summary>
/// The ShellInjectInitializer class is responsible for initializing the dependency injection container
/// and providing access to registered services in a .NET MAUI application. This class implements
/// the IMauiInitializeService interface to be invoked during the app's initialization phase.
/// </summary>
internal class ShellInjectInitializer : IMauiInitializeService
{
    /// <summary>
    /// A static variable holding the root <see cref="IServiceProvider"/> instance
    /// used for resolving dependencies in the ShellInject dependency injection system.
    /// This variable is initialized during the application startup phase.
    /// </summary>
    internal static IServiceProvider? ServiceProvider;

    /// <summary>
    /// Retrieves a service of the specified type from the DI container.
    /// Throws an <see cref="InvalidOperationException"/> if the service provider has not been initialized
    /// or if the requested service of type <typeparamref name="T"/> cannot be resolved.
    /// </summary>
    /// <typeparam name="T">The type of service to resolve.</typeparam>
    /// <returns>The requested service instance of type <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the service provider is null, indicating that the dependency injection system
    /// has not been properly initialized or configured.
    /// </exception>
    internal static T GetRequiredService<T>() where T : notnull
    {
        if (ServiceProvider is null)
        {
            throw new InvalidOperationException("ShellInject has not been constructed or is not properly configured.");
        }

        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Retrieves a registered service of the specified type from the dependency injection system.
    /// Throws an <see cref="InvalidOperationException"/> if the service provider is uninitialized
    /// or if the specified service of type <typeparamref name="T"/> has not been registered.
    /// </summary>
    /// <typeparam name="T">The type of the service to resolve.</typeparam>
    /// <returns>The resolved service instance of type <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the service provider has not been initialized or if a service for the specified
    /// type <typeparamref name="T"/> is not registered in the DI container.
    /// </exception>
    internal static T GetService<T>() where T : notnull
    {
        if (ServiceProvider is null)
        {
            throw new InvalidOperationException("ShellInject has not been constructed or is not properly configured.");
        }

        var service = ServiceProvider.GetService<T>();
        if (service is null)
        {
            throw new InvalidOperationException($"No service for type {typeof(T).Name} has been registered.");
        }

        return service;
    }

    // NOTE:  -Called automatically by MAUI after the final service provider is built:
    /// <summary>
    /// Initializes the dependency injection system for the application by assigning the provided service provider
    /// to the static <see cref="ServiceProvider"/> property.
    /// </summary>
    /// <param name="services">The <see cref="IServiceProvider"/> instance that contains the registered services.</param>
    public void Initialize(IServiceProvider services)
    {
        ServiceProvider = services; 
    }
}