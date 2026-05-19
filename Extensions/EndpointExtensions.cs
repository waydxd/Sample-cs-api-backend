namespace TodoApi.Extensions;

/// <summary>
/// Defines a contract for endpoint modules that register their own routes.
/// Implementations are auto-discovered via reflection by <see cref="EndpointExtensions.RegisterAllEndpoints"/>.
/// </summary>
public interface IEndpointModule
{
    /// <summary>Regroutes all API endpoints for this module.</summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    void RegisterEndpoints(IEndpointRouteBuilder endpoints);
}

/// <summary>Extension methods for registering endpoint modules via reflection.</summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Discovers all types implementing <see cref="IEndpointModule"/> in the assembly,
    /// instantiates them, and calls <see cref="IEndpointModule.RegisterEndpoints"/> on each.
    /// This keeps Program.cs clean by avoiding explicit endpoint registration.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The same <paramref name="endpoints"/> instance for chaining.</returns>
    public static IEndpointRouteBuilder RegisterAllEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var modules = typeof(EndpointExtensions).Assembly.GetTypes()
            .Where(t => typeof(IEndpointModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var module in modules)
        {
            var instance = (IEndpointModule)Activator.CreateInstance(module)!;
            instance.RegisterEndpoints(endpoints);
        }

        return endpoints;
    }
}
