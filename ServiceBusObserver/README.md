# Service Bus Observer

This is a testing package for Azure Service Bus with an observer pattern.

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

    public LogMessageHandler() : base()
    {
        
    }
    public LogMessageHandler() : base(ServiceBusConstants.DemoQueue)
    {
    }

    public override Task HandleMessageAsync(ServiceBusReceivedMessage message)
    {
        // your logic here - what to do with the message        
        return Task.CompletedTask;
    }
}
```