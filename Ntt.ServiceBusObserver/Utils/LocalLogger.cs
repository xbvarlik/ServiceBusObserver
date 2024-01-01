using Microsoft.Extensions.Logging;

namespace Ntt.ServiceBusObserver.Utils;

public static class LocalLogger
{
    public static ILogger<T> CreateLogger<T>()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var logger = loggerFactory.CreateLogger<T>();
        return logger;
    }
}