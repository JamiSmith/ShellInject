namespace ShellInject.Services;

public static class ShellInjectService
{
    public static TService? GetRequiredService<TService>() where TService : class
    {
        return ShellInjectMauiBuilderExtensions.ServiceProvider is null 
            ? null 
            : ShellInjectMauiBuilderExtensions.ServiceProvider.GetService<TService>();
    }


}