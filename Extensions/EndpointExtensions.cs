namespace TodoApi.Extensions;

public interface IEndpointModule
{
    void RegisterEndpoints(IEndpointRouteBuilder endpoints);
}

public static class EndpointExtensions
{
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
