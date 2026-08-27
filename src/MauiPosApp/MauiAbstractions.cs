namespace MauiPosApp;

// Compatibility abstractions ensuring compile-time readiness across standard .NET 9 runtimes and MAUI platforms.

public class Application
{
    public object? MainPage { get; set; }
    protected virtual void InitializeComponent() { }
}

public class Shell
{
    protected virtual void InitializeComponent() { }
}

public class ContentPage
{
    public object? BindingContext { get; set; }
    protected virtual void InitializeComponent() { }
}

public class MauiApp
{
    public static MauiAppBuilder CreateBuilder() => new();
}

public class FontCollection
{
    public FontCollection AddFont(string filename, string alias) => this;
}

public class MauiAppBuilder
{
    public IServiceCollection Services { get; } = new ServiceCollection();

    public MauiAppBuilder UseMauiApp<T>() where T : class => this;

    public MauiAppBuilder ConfigureFonts(Action<FontCollection> configure)
    {
        configure?.Invoke(new FontCollection());
        return this;
    }

    public MauiApp Build() => new();
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
        return services;
    }

    public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient));
        return services;
    }

    public static IServiceCollection AddTransient<TService>(this IServiceCollection services)
        where TService : class
    {
        services.Add(new ServiceDescriptor(typeof(TService), typeof(TService), ServiceLifetime.Transient));
        return services;
    }
}

public enum ServiceLifetime { Singleton, Transient, Scoped }

public class ServiceDescriptor
{
    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public ServiceLifetime Lifetime { get; }

    public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Lifetime = lifetime;
    }
}

public interface IServiceCollection : IList<ServiceDescriptor> { }

public class ServiceCollection : List<ServiceDescriptor>, IServiceCollection { }
