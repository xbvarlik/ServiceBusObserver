namespace Ntt.ServiceBusObserver.Publisher;

public abstract class Observer : IObserver
{
    public virtual QueuePublisherService _service { get; set; } = null!;
    public string queueName { get; set; } = null!;
    
    protected Observer()
    {
        
    }

    protected Observer(QueuePublisherService service, string queueName) 
    {
        this.queueName = queueName;
        _service = service;
    }
    
    public virtual async Task OnEventOccured<T>(T message)
    {
        await _service.SendMessageAsync(message, queueName);
    }
}