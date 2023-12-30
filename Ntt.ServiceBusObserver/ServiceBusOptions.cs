namespace Ntt.ServiceBusObserver;

public class ServiceBusOptions
{
    public string ConnectionString { get; set; } = null!;
    public List<string> Queues { get; set; } = new List<string>();
}