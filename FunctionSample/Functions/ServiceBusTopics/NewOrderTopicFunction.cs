using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionSample.Functions.ServiceBusTopics;

public class NewOrderTopicFunction(ILogger<NewOrderTopicFunction> logger)
{
    private readonly ILogger<NewOrderTopicFunction> _logger = logger;

    [Function(nameof(NewOrderTopicFunction))]
    public async Task Run(
        [ServiceBusTrigger("new-order-events", "inventory-sub", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}