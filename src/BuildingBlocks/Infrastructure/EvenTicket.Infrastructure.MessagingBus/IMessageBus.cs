using EvenTicket.Infrastructure.Messages;

namespace EvenTicket.Infrastructure.MessagingBus;

public interface IMessageBus
{
    Task PublishMessage(IntegrationBaseMessage message, string topicName);
}

