using Azure.Messaging.ServiceBus;
using EvenTicket.Infrastructure.MessagingBus;
using EvenTicket.Services.Ordering.Entities;
using EvenTicket.Services.Ordering.Messages;
using EvenTicket.Services.Ordering.Repositories;
using System.Text.Json;

namespace EvenTicket.Services.Ordering.Messaging;

public class AzServiceBusConsumer : IAzServiceBusConsumer
{
    private readonly IConfiguration _configuration;
    private readonly OrderRepository _orderRepository;
    private readonly IMessageBus _messageBus;

    private readonly string _checkoutMessageTopic;
    private readonly string _orderPaymentUpdatedMessageTopic;
    private readonly string _orderPaymentRequestTopic;

    private readonly ServiceBusProcessor _checkoutMessageProcessor;
    private readonly ServiceBusProcessor _orderPaymentUpdateProcessor;

    public AzServiceBusConsumer(IConfiguration configuration, IMessageBus messageBus, OrderRepository orderRepository)
    {
        _configuration = configuration;
        _orderRepository = orderRepository;
        _messageBus = messageBus;

        var checkoutSubscription = _configuration.GetValue<string>("CheckoutMessageSubscription");
        var orderPaymentRequestSubscription = _configuration.GetValue<string>("OrderPaymentRequestSubscription");
        var orderPaymentUpdatedSubscription = _configuration.GetValue<string>("OrderPaymentUpdatedSubscription");

        var serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        

        _checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
        _orderPaymentUpdatedMessageTopic = _configuration.GetValue<string>("OrderPaymentUpdatedMessageTopic");
        _orderPaymentRequestTopic = _configuration.GetValue<string>("OrderPaymentRequestMessageTopic");


        var client = new ServiceBusClient(serviceBusConnectionString);
        _checkoutMessageProcessor = client.CreateProcessor(_checkoutMessageTopic, checkoutSubscription, new ServiceBusProcessorOptions());
        _orderPaymentUpdateProcessor = client.CreateProcessor(_orderPaymentUpdatedMessageTopic, orderPaymentUpdatedSubscription, new ServiceBusProcessorOptions());
    }

    public void Start()
    {
        _checkoutMessageProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
        _checkoutMessageProcessor.ProcessErrorAsync += OnServiceBusException;
        _checkoutMessageProcessor.StartProcessingAsync();


        _orderPaymentUpdateProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        _orderPaymentUpdateProcessor.ProcessErrorAsync += OnServiceBusException;
        _orderPaymentUpdateProcessor.StartProcessingAsync();
    }

    private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            var basketCheckoutMessage = JsonSerializer.Deserialize<BasketCheckoutMessage>(body);

            Guid orderId = Guid.NewGuid();

            var order = new Order
            {
                UserId = basketCheckoutMessage.UserId,
                Id = orderId,
                OrderPaid = false,
                OrderPlaced = DateTime.Now,
                OrderTotal = basketCheckoutMessage.BasketTotal
            };

            await _orderRepository.AddOrder(order);

            var orderPaymentRequestMessage = new OrderPaymentRequestMessage
            {
                CardExpiration = basketCheckoutMessage.CardExpiration,
                CardName = basketCheckoutMessage.CardName,
                CardNumber = basketCheckoutMessage.CardNumber,
                OrderId = orderId,
                Total = basketCheckoutMessage.BasketTotal
            };
            orderPaymentRequestMessage.CreationDateTime = DateTime.UtcNow;
            orderPaymentRequestMessage.Id = Guid.NewGuid();

            await _messageBus.PublishMessage(orderPaymentRequestMessage, _orderPaymentRequestTopic);

            // Complete the message only after successful processing
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            // Log the exception and optionally dead-letter the message
            Console.WriteLine($"Error processing message: {ex.Message}");

            // Optionally move the message to the dead-letter queue
            await args.DeadLetterMessageAsync(args.Message, "ProcessingFailed", ex.Message);
        }
    }

    private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var orderPaymentUpdateMessage = JsonSerializer.Deserialize<OrderPaymentUpdateMessage>(body);

        await _orderRepository.UpdateOrderPaymentStatus(orderPaymentUpdateMessage.OrderId, orderPaymentUpdateMessage.PaymentSuccess);

        await args.CompleteMessageAsync(args.Message);
    }

    private Task OnServiceBusException(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Message handler encountered an exception: {args.Exception.Message}");
        return Task.CompletedTask;
    }

    public async void Stop()
    {
        await _checkoutMessageProcessor.StopProcessingAsync();
        await _orderPaymentUpdateProcessor.StopProcessingAsync();
    }
}