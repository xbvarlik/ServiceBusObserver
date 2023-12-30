namespace Ntt.ServiceBusObserver.Publisher;

public class Subject
{
    private IList<IObserver> Observers { get; } = new List<IObserver>();
    
    public async Task NotifyObserversAsync<T>(T message)
    {
        foreach (var observer in Observers)
        {
            await observer.OnEventOccured(message);
        }
    }
    
    public void RegisterObserver(IObserver observer)
    {
        Observers.Add(observer);
    }
    
    public void UnregisterObserver(IObserver observer)
    {
        Observers.Remove(observer);
    }
}