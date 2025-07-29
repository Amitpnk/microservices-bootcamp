using System.Text;
using EvenTicket.Infrastructure.MessagingBus;
using EvenTicket.Services.Payment.Messages;
using EvenTicket.Services.Payment.Model;
using EvenTicket.Services.Payment.Services;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace EvenTicket.Services.Payment.Worker;

public class ServiceBusListener : IHostedService
{
    private readonly ILogger logger;
    private readonly IConfiguration configuration;
    private ServiceBusProcessor processor;
    private readonly IExternalGatewayPaymentService externalGatewayPaymentService;
    private readonly IMessageBus messageBus;
    private readonly string orderPaymentUpdatedMessageTopic;
    private ServiceBusClient serviceBusClient;


    public ServiceBusListener(IConfiguration configuration, ILoggerFactory loggerFactory, IExternalGatewayPaymentService externalGatewayPaymentService, IMessageBus messageBus)
    {
        logger = loggerFactory.CreateLogger<ServiceBusListener>();
        orderPaymentUpdatedMessageTopic = configuration.GetValue<string>("OrderPaymentUpdatedMessageTopic");

        this.configuration = configuration;
        this.externalGatewayPaymentService = externalGatewayPaymentService;
        this.messageBus = messageBus;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string connectionString = configuration.GetValue<string>("ServiceBusConnectionString");
        string topic = configuration.GetValue<string>("OrderPaymentRequestMessageTopic");
        string subscription = configuration.GetValue<string>("OrderPaymentRequestSubscription");



        serviceBusClient = new ServiceBusClient(connectionString);
        processor = serviceBusClient.CreateProcessor(topic, subscription, new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 3,
            AutoCompleteMessages = false
        });

        processor.ProcessMessageAsync += ProcessMessageAsync;
        processor.ProcessErrorAsync += ProcessErrorAsync;

        await processor.StartProcessingAsync(cancellationToken);
    }


    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug($"ServiceBusListener stopping.");
        if (processor != null)
        {
            await processor.StopProcessingAsync(cancellationToken);
            await processor.DisposeAsync();
        }
        if (serviceBusClient != null)
        {
            await serviceBusClient.DisposeAsync();
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error while processing queue item in ServiceBusListener.");
        return Task.CompletedTask;
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var messageBody = args.Message.Body.ToString();
        OrderPaymentRequestMessage orderPaymentRequestMessage = JsonConvert.DeserializeObject<OrderPaymentRequestMessage>(messageBody);

        PaymentInfo paymentInfo = new PaymentInfo
        {
            CardNumber = orderPaymentRequestMessage.CardNumber,
            CardName = orderPaymentRequestMessage.CardName,
            CardExpiration = orderPaymentRequestMessage.CardExpiration,
            Total = orderPaymentRequestMessage.Total
        };

        var result = await externalGatewayPaymentService.PerformPayment(paymentInfo);

        await args.CompleteMessageAsync(args.Message);

        //send payment result to order service via service bus
        OrderPaymentUpdateMessage orderPaymentUpdateMessage = new OrderPaymentUpdateMessage
        {
            PaymentSuccess = result,
            OrderId = orderPaymentRequestMessage.OrderId
        };

        try
        {
            await messageBus.PublishMessage(orderPaymentUpdateMessage, orderPaymentUpdatedMessageTopic);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        logger.LogDebug($"{orderPaymentRequestMessage.OrderId}: ServiceBusListener received item.");
        //await Task.Delay(20000);
        //logger.LogDebug($"{orderPaymentRequestMessage.OrderId}:  ServiceBusListener processed item.");
    }
}