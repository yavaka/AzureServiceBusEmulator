using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionSample.Functions.ServiceBusQueues;

public class ProcessOrderQueueFunction(ILogger<ProcessOrderQueueFunction> logger)
{
    private readonly ILogger<ProcessOrderQueueFunction> _logger = logger;

    [Function(nameof(ProcessOrderQueueFunction))]
    public async Task Run(
        [ServiceBusTrigger("process-order-events", Connection = "ServiceBusConnectionString")]
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