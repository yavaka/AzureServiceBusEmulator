using Azure.Messaging.ServiceBus;
using FunctionSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FunctionSample.Functions.Http;

public class NewOrderFunction(
    ILogger<NewOrderFunction> logger,
    ServiceBusClient serviceBusClient)
{
    private readonly ILogger<NewOrderFunction> _logger = logger;
    private readonly ServiceBusClient _serviceBusClient = serviceBusClient;

    [Function(nameof(NewOrderFunction))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put")] HttpRequestData req)
    {
        ArgumentNullException.ThrowIfNull(req);

        var orderModel = await req.ReadFromJsonAsync<OrderModel>();
        ArgumentNullException.ThrowIfNull(orderModel);

        _logger.LogInformation("{functionName} HTTP trigger function processing an order {orderId}.", nameof(NewOrderFunction), orderModel.OrderId);

        var sender = this._serviceBusClient.CreateSender("new-order-events");
        var message = new ServiceBusMessage(JsonSerializer.Serialize(orderModel));
        await sender.SendMessageAsync(message);

        _logger.LogInformation("{functionName} HTTP trigger function processed an order {orderId}.", nameof(NewOrderFunction), orderModel.OrderId);

        return new AcceptedResult("new-order-events", orderModel);
    }
}