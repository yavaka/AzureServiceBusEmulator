# FunctionSample

Lightweight Azure Functions sample demonstrating an HTTP-triggered producer that publishes orders to an Azure Service Bus Topic and a topic subscriber that processes those orders. Includes local configuration and an Azure Service Bus Emulator configuration for local development.

## Key points
- .NET target: .NET 9 (isolated worker)
- Language: C# 13
- Patterns: HTTP -> Service Bus Topic -> Topic Subscription consumer
- Topic: `new-order-events`
- Subscriptions (example): `inventory-sub`, `email-sub`

## Project layout
- Functions/
  - Http/NewOrderFunction.cs — HTTP PUT endpoint that sends an OrderModel to Service Bus topic `new-order-events`.
  - ServiceBusTopics/NewOrderTopicFunction.cs — ServiceBusTrigger on topic `new-order-events` and subscription `inventory-sub`.
- Models/OrderModel.cs — simple order DTO used by functions.
- Program.cs — DI, Application Insights and Service Bus client registration.
- host.json — Functions host logging / Application Insights config.
- local.settings.json — local function app settings (connection string shown for emulator usage).
- AzureServiceBusEmulator/config.json — emulator namespace, topics, and subscriptions used for local testing.
- AzureServiceBusEmulator/.env — environment variables for the emulator docker-compose.

## Prerequisites
- .NET 9 SDK
- Azure Functions Core Tools (for local function host) compatible with dotnet-isolated
- Docker & Docker Compose (to run the Service Bus emulator)
- Visual Studio 2022 or your preferred editor/IDE

## Local setup (recommended)
1. Start the Service Bus emulator (see the emulator sample README in `../EmulatorSample/README.md` or use the provided `AzureServiceBusEmulator/config.json` and `.env`).
   - Typically you will set env vars and run:
     - (PowerShell example from Emulator sample)
     - Set __CONFIG_PATH__, __MSSQL_SA_PASSWORD__, and __ACCEPT_EULA__ then `docker-compose up -d`
2. Verify emulator exposes Service Bus endpoint and the topic/subscriptions exist:
   - Topic: `new-order-events`
   - Subscriptions: `inventory-sub`, `email-sub`
   - The repository includes `AzureServiceBusEmulator/config.json` that creates those.

3. Configure local.settings.json
   - By default local.settings.json contains:
     {
       "IsEncrypted": false,
       "Values": {
         "AzureWebJobsStorage": "UseDevelopmentStorage=true",
         "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
         "ServiceBusConnectionString": "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;"
       }
     }
   - Replace SAS_KEY_VALUE or update the connection string as needed for your emulator or Azure Service Bus namespace.

4. Run the Functions app
   - From terminal in function project root:
     - dotnet build
     - func start
   - Or open solution in Visual Studio 2022 and run using __Debug > Start Debugging__ (F5) or __Run > Start Without Debugging__.

## How it works

1. HTTP producer
   - Endpoint: NewOrderFunction (HTTP PUT)
   - Reads an OrderModel from the request JSON and sends it to Service Bus topic `new-order-events`.
   - Example payload:
     {
       "OrderId": 42,
       "ItemName": "Widget",
       "Quantity": 3
     }
   - Example curl:
     curl -X PUT http://localhost:7071/api/NewOrderFunction -H "Content-Type: application/json" -d '{"OrderId":42,"ItemName":"Widget","Quantity":3}'

2. Topic subscriber
   - NewOrderTopicFunction is triggered by messages on topic `new-order-events`, subscription `inventory-sub`.
   - It logs message metadata and completes the message via MessageActions.

## Files to review / edit
- Program.cs — registers ServiceBusClient using the `ServiceBusConnectionString` configuration key.
- local.settings.json — set local Service Bus connection string or point to emulator.
- AzureServiceBusEmulator/config.json — edit to add more topics/subscriptions when using the emulator.

## Notes and tips
- The sample uses the Azure.Messaging.ServiceBus client and the isolated worker model for Functions.
- Messages are explicitly completed by the subscriber (`messageActions.CompleteMessageAsync(message)`).
- For real Azure usage, replace the emulator connection string with a real Azure Service Bus connection string and review security practices for storing secrets (use Key Vault / managed identity in production).
- If you need to debug locally, open the project in Visual Studio 2022 and use __Debug > Start Debugging__; ensure the emulator is running before sending requests.

## Troubleshooting
- "Cannot connect" to Service Bus — confirm emulator container is up, ports open, and connection string matches emulator settings.
- "Missing ServiceBusConnectionString" — ensure `local.settings.json` or environment variables provide the `ServiceBusConnectionString` key used in Program.cs.
- Use the Output window in Visual Studio to inspect function logs, or run `func start` in a terminal to see console logs.

## License
Sample provided as-is for learning and local development.
