using System.Diagnostics.CodeAnalysis;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ntt.ServiceBusObserver.Utils;

namespace Ntt.ServiceBusObserver.Consumer;

public class QueueReceiverHostedService(ServiceBusOptions options, QueueHandlerRegistry handlerRegistry) : BackgroundService
{
    private readonly ServiceBusClient _client = new (options.ConnectionString);
    private readonly IList<string>? _queueNames = options.Queues;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ExecuteMessageQueues(stoppingToken);
        }
    }
    
    private async Task ExecuteMessageQueues(CancellationToken stoppingToken)
    {
        if (_queueNames == null || !_queueNames.Any())
            return;
        
        var receiver = new QueueMessageReceiver(_client);
        var logger = LocalLogger.CreateLogger<QueueReceiverHostedService>();
        
        var tasks = _queueNames.Select(async queueName =>
        {
            try
            {
                var handler = handlerRegistry.GetHandler(queueName);
                await receiver.ReceiveMessageBatchFromQueue(queueName, handler.HandleMessageAsync, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                logger.LogError(e.Message);
            }
        });

        await Task.WhenAll(tasks);
    }
}

internal class QueueMessageReceiver(ServiceBusClient client)
{
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public async Task ReceiveMessageBatchFromQueue(string queueName,
        Func<ServiceBusReceivedMessage, Task> messageHandler,
        CancellationToken cancellationToken = default)
    {
        var receiver = client.CreateReceiver(queueName);
        var messages = await receiver.ReceiveMessagesAsync(1000, cancellationToken: cancellationToken);
        
        if (!messages.Any()) 
            return;
        
        var logger = LocalLogger.CreateLogger<QueueMessageReceiver>();

        foreach (var message in messages)
        {
            try
            {
                await messageHandler(message);
                await receiver.CompleteMessageAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                await receiver.AbandonMessageAsync(message, null, cancellationToken);
                logger.LogError(ex.Message);
            }
        }
    }
}