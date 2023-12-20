namespace ServiceBusObserver.Consumer;

public class QueueHandlerRegistry
{
    private IDictionary<string, IMessageHandler> Handlers { get; set; } = new Dictionary<string, IMessageHandler>();

    public void RegisterHandler(string queueName, IMessageHandler handler) 
        => Handlers[queueName] = handler;

    public IMessageHandler GetHandler(string queueName)
    {
        if (Handlers.TryGetValue(queueName, out var handler))
            return handler;
        
        throw new KeyNotFoundException($"No handler registered for queue: {queueName}");
    }
}
