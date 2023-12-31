# Service Bus Observer

This is a testing package for Azure Service Bus with an observer pattern.
It is advised that you use same message type for both publisher and subscriber and deserialize it in the handler.
It does not have to be any specific type, but it must be the same in both ends.

Requires .NET 8.0 or higher.

## Prerequisites

Add the below configuration to your appsettings.json or appsettings.development.json file:

```json
{
  "ServiceBusOptions": {
    "ConnectionString": "your-service-bus-connection-string",
    "Queues": {
      "YourQueueName": "your-queue-name"
    }
  }
}
```

## Usage

### For publisher (sender):

#### Program.cs

```csharp
    builder.Services.AddPublisher(builder.Configuration);
        
    builder.Services.AddScoped<YourSubjectClass>(sp => 
        sp.AddSubject<YourSubjectClass>()
            .AddObserver<YourSubjectClass, YourObserverClass>(sp)
        );
```
#### Example Subject

```csharp
public class YourSubjectClass : Subject
{
    
}
```

#### Example Observer

```csharp
public class YourObserverClass : Observer
{
    public YourObserverClass() 
    {
    }
    public YourObserverClass(QueuePublisherService service) : base(service, YourQueueName)
    {
    }
}
```

#### Example Controller
```csharp
[ApiController]
[Route("api/[controller]")]
public class YourController(TriggerSubject triggerSubject /*, any other service you use*/) : ControllerBase
{    
    [HttpMethod]
    public async Task<IActionResult> CreateTriggerModelAsync(YourModel yourModel)
    {
        var model = /* your logic that returns any model */;

        await triggerSubject.NotifyObserversAsync(model);
    }
}
```

### For subscriber (receiver):

```csharp
    builder.Services.AddSingleton<QueueHandlerRegistry>(sp => 
            sp.AddHandlerRegistry().
                AddMessageHandler<YourMessageHandler>(sp));
        
    builder.Services.AddSubscriber(builder.Configuration);
```

#### Example Handler

```csharp
public class YourMessageHandler : MessageHandler
{
    public override string QueueName { get; set; }
    public override IServiceScopeFactory ScopeFactory { get; set; }

    public YourMessageHandler() : base()
    {
        
    }
    public YourMessageHandler(IServiceScopeFactory scopeFactory) : base(scopeFactory, ServiceBusConstants.DemoQueue)
    {
    }

    public override Task HandleMessageAsync(ServiceBusReceivedMessage message)
    {
        // you can use the below line to get any service you need
        var service = ServiceBundleUtility.GetService<YourService>(ScopeFactory);
        
        var deserializedMessage = DeserializeMessage<YourType>(message);
        // your logic here - what to do with the message        
        return Task.CompletedTask;
    }
}
```
