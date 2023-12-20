using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusObserver.Consumer;
using ServiceBusObserver.Publisher;

namespace ServiceBusObserver;

public static class Bootstrapper
{
    public static void AddSubscriber(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusOptions = configuration.GetSection("ServiceBusOptions").Get<ServiceBusOptions>();

        if (serviceBusOptions == null)
            throw new InvalidOperationException("ServiceBusOptions not found in configuration");
        
        services.AddHostedService(serviceProvider => 
            new QueueReceiverHostedService(
                serviceBusOptions, 
                serviceProvider.GetRequiredService<QueueHandlerRegistry>())
        );
    }

    public static QueueHandlerRegistry AddHandlerRegistry(this IServiceProvider services)
    {
        var registry = new QueueHandlerRegistry();
        return registry;
    }
    
    public static QueueHandlerRegistry AddMessageHandler<TMessageHandler>(this QueueHandlerRegistry registry, IServiceProvider services)
    where TMessageHandler : IMessageHandler, new()
    {
        var messageHandler = ActivatorUtilities.CreateInstance<TMessageHandler>(services);
        registry.RegisterHandler(messageHandler.QueueName, messageHandler);
        return registry;
    }
    
    public static void AddPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusOptions = configuration.GetSection("ServiceBusOptions").Get<ServiceBusOptions>();

        if (serviceBusOptions == null)
            throw new InvalidOperationException("ServiceBusOptions not found in configuration");

        services.AddScoped(_ => new QueuePublisherService(serviceBusOptions));
    }
    
    public static TSubject AddSubject<TSubject>(this IServiceProvider serviceProvider) 
        where TSubject : Subject
    {
        var subject = ActivatorUtilities.CreateInstance<TSubject>(serviceProvider);
        return subject;
    }

    public static TSubject AddObserver<TSubject, TObserver>(this TSubject subject, IServiceProvider serviceProvider)
        where TObserver : IObserver, new()
        where TSubject : Subject
    {
        var observer = ActivatorUtilities.CreateInstance<TObserver>(serviceProvider);
        subject.RegisterObserver(observer);
        return subject;
    }
}