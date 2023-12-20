using Azure.Messaging.ServiceBus;

namespace ServiceBusObserver.Consumer;

public interface IMessageHandler
{
    string QueueName { get; set; }
    Task HandleMessageAsync(ServiceBusReceivedMessage message);
}

public abstract class MessageHandler : IMessageHandler
{
    protected MessageHandler()
    {
        
    }

    protected MessageHandler(string queueName)
    {
        QueueName = queueName;
    }
    
    public virtual string QueueName { get; set; } = null!;
    public abstract Task HandleMessageAsync(ServiceBusReceivedMessage message);
}