using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;

namespace ServiceBusObserver.Consumer;

public class QueueReceiverHostedService(ServiceBusOptions options, QueueHandlerRegistry handlerRegistry) : BackgroundService
{
    private readonly ServiceBusClient _client = new (options.ConnectionString);
    private readonly IList<string>? _queueNames = options.Queues;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ExecuteMessageQueues(stoppingToken);
            
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ExecuteMessageQueues(CancellationToken stoppingToken)
    {
        if (_queueNames == null || !_queueNames.Any())
            await StopAsync(stoppingToken);
        
        var receiver = new QueueMessageReceiver(_client);
        
        foreach (var queueName in _queueNames!)
        {
            var handler = handlerRegistry.GetHandler(queueName);
            await receiver.ReceiveMessagesFromQueueAsync(queueName, handler.HandleMessageAsync, stoppingToken);
        }
    }
}

internal class QueueMessageReceiver(ServiceBusClient client)
{
    public async Task ReceiveMessagesFromQueueAsync(string queueName, Func<ServiceBusReceivedMessage, Task> messageHandler, CancellationToken cancellationToken)
    {
        var receiver = client.CreateReceiver(queueName);
        var message = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);

        if (message == null) 
            return;
            
        try
        {
            await messageHandler(message);
            await receiver.CompleteMessageAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            await receiver.AbandonMessageAsync(message, null, cancellationToken);
            throw new Exception($"Error occurred while processing message: {message.Body}", ex);
        }
    }
}