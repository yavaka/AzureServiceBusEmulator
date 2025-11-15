# Azure Service Bus Emulator Sample

This project demonstrates how to use the **Azure Service Bus Emulator** with a .NET console application. It showcases both queue and topic-based messaging patterns using the Azure Service Bus client library.

## Overview

The project includes examples of:
- **Queue-based messaging**: Send and receive messages from a Service Bus queue
- **Topic-based messaging**: Publish messages to a topic and consume them from multiple subscriptions with correlation filters

## Prerequisites

- **.NET 10**
- **Docker** and **Docker Compose** (for running the Azure Service Bus Emulator)
- **Azure Service Bus NuGet package** (v7.20.1+)

## Project Structure

```
.
├── Program.cs                 # Main application with messaging examples
├── config.json               # Service Bus Emulator configuration
├── docker-compose.yaml       # Docker Compose setup for the emulator
└── README.md                 # This file
```

## Getting Started

### 1. Start the Service Bus Emulator

Using Docker Compose:

```powershell
# Set required environment variables
$env:CONFIG_PATH = "$(Get-Location)\config.json"
$env:MSSQL_SA_PASSWORD = "YourStrongPassword123!"
$env:ACCEPT_EULA = "Y"
$env:SQL_WAIT_INTERVAL = "30"

# Start the emulator and SQL Server
docker-compose up -d
```

The emulator will be available at: `localhost` (Port 5672 for AMQP, Port 5300 for HTTP)

### 2. Configure the Connection String

The default connection string in `Program.cs` is configured for the local emulator:

```
Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;
```

### 3. Run the Application

```powershell
dotnet run Program.cs
```

## What the Application Does

### Case 1: Queue Messaging

The application sends 100 messages (10 batches × 10 messages) to a queue and then receives them:

- **Send**: Publishes messages to `queue.1`
- **Receive**: Consumes messages from `queue.1` using PeekLock mode

```
Output:
10 batches with 10 messages per batch has been published to the queue.
Received message: Batch:1:Message:1
...
```

### Case 2: Topic-Based Messaging

The application publishes messages to a topic with multiple subscriptions using correlation filters:

- **Publish**: Sends messages to `topic.1`
- **Subscribe**: Consumes messages from subscriptions with:
  - `subscription.1`: Filters by ContentType and CorrelationId
  - `subscription.2` & `subscription.3`: Additional subscriptions with their own filters

## Configuration

The `config.json` file defines:

- **Queue Properties**: Message TTL, lock duration, max delivery count, duplicate detection
- **Topic Configuration**: Multiple subscriptions with correlation-based filters
- **Dead Letter Handling**: Configuration for expired messages and failed deliveries

Example queue configuration:
```json
{
  "Name": "queue.1",
  "Properties": {
    "DefaultMessageTimeToLive": "PT1H",
    "LockDuration": "PT1M",
    "MaxDeliveryCount": 3,
    "RequiresDuplicateDetection": false
  }
}
```

## Docker Compose Services

### Service Bus Emulator
- **Image**: `mcr.microsoft.com/azure-messaging/servicebus-emulator:latest`
- **Container**: `servicebus-emulator`
- **Ports**: 5672 (AMQP), 5300 (HTTP)
- **Configuration**: Loads `config.json` for queue/topic definitions

### SQL Server 2022
- **Image**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Container**: `mssql`
- **Purpose**: Backend database for the Service Bus Emulator
- **Note**: Password must meet SQL Server strong password policy

## Receive Modes

The application uses **PeekLock** mode for receiving messages:

- **PeekLock**: Messages are locked for processing and must be explicitly completed or abandoned
- Alternative: **ReceiveAndDelete** - Messages are automatically deleted upon receipt

## Key Features

✅ Batch message processing for efficient publishing  
✅ Asynchronous messaging with `async/await`  
✅ Correlation filters for topic subscriptions  
✅ Proper resource cleanup with `DisposeAsync()`  
✅ Error handling with message timeouts  
✅ Local development without cloud costs  

## Cleanup

To stop and remove the emulator and database containers:

```powershell
docker-compose down
```

To remove volumes as well:

```powershell
docker-compose down -v
```

## Next Steps

- Modify `config.json` to add more queues, topics, or subscriptions
- Implement custom message processing logic in `ConsumeMessageFromDefaultQueue()`
- Add message properties and headers for advanced scenarios
- Integrate with your own application logic
- Move to Azure Service Bus cloud service when ready for production

## Resources

- [Azure Service Bus Documentation](https://learn.microsoft.com/en-us/azure/service-bus-messaging/)
- [Azure Service Bus Emulator](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-emulator-overview)
- [Azure SDK for .NET - Service Bus](https://learn.microsoft.com/en-us/dotnet/api/azure.messaging.servicebus)
- [Docker](https://www.docker.com/)

## License

This sample is provided as-is for educational purposes.
