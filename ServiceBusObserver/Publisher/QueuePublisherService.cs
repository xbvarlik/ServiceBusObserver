using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace ServiceBusObserver.Publisher;

public class QueuePublisherService(ServiceBusOptions options)
{
    private readonly ServiceBusClient _client = new(options.ConnectionString);
    
    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        var sender = _client.CreateSender(queueName);
        var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message));
        await sender.SendMessageAsync(serviceBusMessage);
    }
    
    public async Task SendMessageBatchAsync<T>(IEnumerable<T> messages, string queueName)
    {
        var sender = _client.CreateSender(queueName);
        var serviceBusMessages = messages.Select(message => 
            new ServiceBusMessage(JsonSerializer.Serialize(message)));
        await sender.SendMessagesAsync(serviceBusMessages);
    }
}