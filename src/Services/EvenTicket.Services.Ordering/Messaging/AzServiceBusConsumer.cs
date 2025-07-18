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
        //var serviceBusConnectionString = serviceBusSettings.Value.ConnectionString;

        _checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
        _orderPaymentUpdatedMessageTopic = _configuration.GetValue<string>("OrderPaymentUpdatedMessageTopic");



        var client = new ServiceBusClient(serviceBusConnectionString);
        _checkoutMessageProcessor = client.CreateProcessor(_checkoutMessageTopic, checkoutSubscription, new ServiceBusProcessorOptions());
        _orderPaymentUpdateProcessor = client.CreateProcessor(_orderPaymentUpdatedMessageTopic, orderPaymentUpdatedSubscription, new ServiceBusProcessorOptions());



        //var subscriptionName = _configuration.GetValue<string>("SubscriptionName");
        //var serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        //_checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
        //_orderPaymentUpdatedMessageTopic = _configuration.GetValue<string>("OrderPaymentUpdatedMessageTopic");

        //var client = new ServiceBusClient(serviceBusConnectionString);

        //_checkoutMessageProcessor = client.CreateProcessor(_checkoutMessageTopic, subscriptionName, new ServiceBusProcessorOptions());
        //_orderPaymentUpdateProcessor = client.CreateProcessor(_orderPaymentUpdatedMessageTopic, subscriptionName, new ServiceBusProcessorOptions());
    }

    public void Start()
    {
        _checkoutMessageProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
        _checkoutMessageProcessor.ProcessErrorAsync += OnServiceBusException;

        _orderPaymentUpdateProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        _orderPaymentUpdateProcessor.ProcessErrorAsync += OnServiceBusException;

        _checkoutMessageProcessor.StartProcessingAsync();
        _orderPaymentUpdateProcessor.StartProcessingAsync();
    }

    private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
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

        try
        {
            //todo: check values
            await _messageBus.PublishMessage(orderPaymentRequestMessage, _checkoutMessageTopic);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error publishing message: {e.Message}");
            throw;
        }

        await args.CompleteMessageAsync(args.Message);
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