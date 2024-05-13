using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecondApplicationForAzure.Common.Configuration;
using SecondApplicationForAzure.Common.Enums;
using SecondApplicationForAzure.Model;
using SecondApplicationForAzure.Model.DbSets;
using SecondApplicationForAzure.Services.Services.Logs.Models;
using System.Text.Json;

namespace SecondApplicationForAzure.Services.Services.Logs;

public interface ILogService
{
    Task ReadLogAsync(CancellationToken stoppingToken);

    Task LogGetListAsync(string message);

    Task LogAddAsync(string message);

    Task LogDeleteAsync(string message);

    Task LogGetByIdAsync(string message);

    Task LogEditAsync(string message);
}

public class LogService : ILogService
{
    private readonly AzureServiceBusSection _configServiceBus;
    private readonly ILogger<ILogService> _logger;
    private readonly string _appName;
    private readonly SecondAppDbContext _context;

    public LogService(
        SecondAppDbContext context,
        IOptions<AzureServiceBusSection> configServiceBus,
        ILogger<ILogService> logger,
        string appName)
    {
        _context = context;
        _configServiceBus = configServiceBus.Value;
        _logger = logger;
        _appName = appName;
    }

    public async Task LogAddAsync(string message)
    {
        await SendLogAsync(new LogModel(EventTypeEnum.Add, message, _appName));
    }

    public async Task LogDeleteAsync(string message)
    {
        await SendLogAsync(new LogModel(EventTypeEnum.Delete, message, _appName));
    }

    public async Task LogEditAsync(string message)
    {
        await SendLogAsync(new LogModel(EventTypeEnum.Edit, message, _appName));
    }

    public async Task LogGetByIdAsync(string message)
    {
        await SendLogAsync(new LogModel(EventTypeEnum.GetList, message, _appName));
    }

    public async Task LogGetListAsync(string message)
    {
        await SendLogAsync(new LogModel(EventTypeEnum.GetById, message, _appName));
    }

    public async Task ReadLogAsync(CancellationToken stoppingToken)
    {
        // the client that owns the connection and can be used to create senders and receivers
        ServiceBusClient client;

        // the processor that reads and processes messages from the queue
        ServiceBusProcessor processor;

        // The Service Bus client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when messages are being published or read
        // regularly.
        //
        // Set the transport type to AmqpWebSockets so that the ServiceBusClient uses port 443.
        // If you use the default AmqpTcp, make sure that ports 5671 and 5672 are open.

        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        client = new ServiceBusClient(_configServiceBus.ListenNamespaceConnectionString, clientOptions);

        // create a processor that we can use to process the messages
        processor = client.CreateProcessor(_configServiceBus.QueueName, new ServiceBusProcessorOptions());

        // add handler to process messages
        processor.ProcessMessageAsync += MessageHandler;

        // add handler to process any errors
        processor.ProcessErrorAsync += ErrorHandler;

        // start processing
        _ = processor.StartProcessingAsync(stoppingToken);
    }

    // handle received messages
    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        var logModel = JsonSerializer.Deserialize<LogModel>(body);

        if (logModel == null)
        {
            await args.DeadLetterMessageAsync(args.Message);
            return;
        }

        string messageId = args.Message.MessageId.ToString();
        var eventDate = args.Message.EnqueuedTime;

        var dbModel = new Log()
        {
            MessageId = messageId,
            ApppName = logModel.ApppName,
            LogType = logModel.LogType,
            Message = logModel.Message,
            EventDate = eventDate.LocalDateTime,
            CreateDate = DateTime.Now,
        };

        await _context.Logs.AddAsync(dbModel);
        await _context.SaveChangesAsync();

        // complete the message. message is deleted from the queue.
        await args.CompleteMessageAsync(args.Message);
    }

    // handle any errors when receiving messages
    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogInformation(args.Exception.ToString());
        return Task.CompletedTask;
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

        client = new ServiceBusClient(_configServiceBus.SendNamespaceConnectionString, clientOptions);
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