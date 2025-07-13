namespace EvenTicket.Infrastructure.Messages;

public class IntegrationBaseMessage
{
    public Guid Id { get; set; }
    public DateTime CreationDateTime { get; set; }
}