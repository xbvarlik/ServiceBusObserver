namespace Ntt.ServiceBusObserver.Publisher;

public interface IObserver
{
    public QueuePublisherService _service { get; set; }
    
    public string queueName { get; set; }
    
    Task OnEventOccured<T>(T message);
}