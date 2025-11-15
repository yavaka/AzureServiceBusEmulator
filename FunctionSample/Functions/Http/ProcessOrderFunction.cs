using Azure.Messaging.ServiceBus;
using FunctionSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FunctionSample.Functions.Http;

public class ProcessOrderFunction(
    ILogger<ProcessOrderFunction> logger,
    ServiceBusClient serviceBusClient)
{
    private readonly ILogger<ProcessOrderFunction> _logger = logger;
    private readonly ServiceBusClient _serviceBusClient = serviceBusClient;

    [Function(nameof(ProcessOrderFunction))]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        ArgumentNullException.ThrowIfNull(req);

        var orderModel = await req.ReadFromJsonAsync<ProcessOrderModel>();
        ArgumentNullException.ThrowIfNull(orderModel);

        _logger.LogInformation("{functionName} HTTP trigger function processing an order {orderId}.", nameof(ProcessOrderFunction), orderModel.OrderId);

        var sender = this._serviceBusClient.CreateSender("process-order-events");
        var message = new ServiceBusMessage(JsonSerializer.Serialize(orderModel));
        await sender.SendMessageAsync(message);

        return new OkObjectResult("Welcome to Azure Functions!");
    }
}