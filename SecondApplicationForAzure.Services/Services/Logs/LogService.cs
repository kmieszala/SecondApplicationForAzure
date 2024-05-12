using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using SecondApplicationForAzure.Common.Configuration;
using SecondApplicationForAzure.Common.Enums;
using SecondApplicationForAzure.Services.Services.Logs.Models;
using System.Text.Json;

namespace SecondApplicationForAzure.Services.Services.Logs;

public interface ILogService
{
    Task LogGetListAsync(string message);

    Task LogAddAsync(string message);

    Task LogDeleteAsync(string message);

    Task LogGetByIdAsync(string message);

    Task LogEditAsync(string message);
}

public class LogService : ILogService
{
    private readonly AzureServiceBusSection _configServiceBus;

    public LogService(IOptions<AzureServiceBusSection> configServiceBus)
    {
        _configServiceBus = configServiceBus.Value;
    }

    public async Task LogAddAsync(string message)
    {
        await SendLogAsync(new LogModel(EventTypeEnum.Add, message));
    }

    public async Task LogDeleteAsync(string message)
    {
        await SendLogAsync(new LogModel(EventTypeEnum.Delete, message));
    }

    public async Task LogEditAsync(string message)
    {
        await SendLogAsync(new LogModel(EventTypeEnum.Edit, message));
    }

    public async Task LogGetByIdAsync(string message)
    {
        await SendLogAsync(new LogModel(EventTypeEnum.GetList, message));
    }

    public async Task LogGetListAsync(string message)
    {
        await SendLogAsync(new LogModel(EventTypeEnum.GetById, message));
    }

    private async Task SendLogAsync(LogModel message)
    {
        // the client that owns the connection and can be used to create senders and receivers
        ServiceBusClient client;

        // the sender used to publish messages to the queue
        ServiceBusSender sender;

        // The Service Bus client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when messages are being published or read
        // regularly.
        //
        // set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443.
        // If you use the default AmqpTcp, you will need to make sure that the ports 5671 and 5672 are open

        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        client = new ServiceBusClient(_configServiceBus.NamespaceConnectionString, clientOptions);
        sender = client.CreateSender(_configServiceBus.QueueName);

        // create a batch
        using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

        // try adding a message to the batch
        string jsonString = JsonSerializer.Serialize(message);
        if (!messageBatch.TryAddMessage(new ServiceBusMessage(jsonString)))
        {
            throw new Exception($"The message {jsonString} is too large to fit in the batch.");
        }

        try
        {
            // Use the producer client to send the batch of messages to the Service Bus queue
            await sender.SendMessagesAsync(messageBatch);
        }
        finally
        {
            // Calling DisposeAsync on client types is required to ensure that network
            // resources and other unmanaged objects are properly cleaned up.
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}