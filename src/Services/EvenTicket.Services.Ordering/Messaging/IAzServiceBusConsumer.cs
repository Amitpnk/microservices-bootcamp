namespace EvenTicket.Services.Ordering.Messaging;

public interface IAzServiceBusConsumer
{
    void Start();
    void Stop();
}