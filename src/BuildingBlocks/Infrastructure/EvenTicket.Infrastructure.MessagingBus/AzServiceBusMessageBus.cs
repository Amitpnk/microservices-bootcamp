using Eventick.Integration.MessagingBus;
using System.Text;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using EvenTicket.Infrastructure.Messages;
using Microsoft.Extensions.Logging;

namespace EvenTicket.Infrastructure.MessagingBus;

public class AzServiceBusMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly ILogger<AzServiceBusMessageBus> _logger;
    private readonly ServiceBusClient _client;
    public AzServiceBusMessageBus(ILogger<AzServiceBusMessageBus> logger)
    {
        string connectionString =
                "Endpoint=sb://<your-namespace>.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<your_key>";


        _client = new ServiceBusClient(connectionString);
        _logger = logger;
    }
    public async Task PublishMessage(IntegrationBaseMessage message, string topicName)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (string.IsNullOrWhiteSpace(topicName))
        {
            throw new ArgumentException("Topic name cannot be null or empty.", nameof(topicName));
        }

        await using var sender = _client.CreateSender(topicName);

        try
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(serviceBusMessage);
            _logger.LogInformation($"Message sent to topic {topicName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending message to topic {topicName}");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
    }
}
