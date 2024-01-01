using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace Ntt.ServiceBusObserver.Consumer;

public interface IMessageHandler
{
    string QueueName { get; set; }
    Task HandleMessageAsync(ServiceBusReceivedMessage message);
}

public abstract class MessageHandler : IMessageHandler
{
    public virtual string QueueName { get; set; }
    public virtual IServiceScopeFactory? ScopeFactory { get; set; }
    
    public MessageHandler()
    {
        
    }

    public MessageHandler(IServiceScopeFactory scopeFactory, string queueName)
    {
        QueueName = queueName;
        ScopeFactory = scopeFactory;
    }

    public abstract Task HandleMessageAsync(ServiceBusReceivedMessage message);
    
    protected virtual T? DeserializeMessage<T>(ServiceBusReceivedMessage message)
    {
        var messageBody = message.Body.ToString();
        return JsonSerializer.Deserialize<T>(messageBody);
    }
}