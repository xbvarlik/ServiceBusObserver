using Microsoft.Extensions.DependencyInjection;

namespace Ntt.ServiceBusObserver.Utils;

public static class ServiceBundleUtility
{
    public static T GetService<T>(IServiceScopeFactory scopeFactory)
        where T : class
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}