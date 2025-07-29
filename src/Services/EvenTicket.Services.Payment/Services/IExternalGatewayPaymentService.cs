using EvenTicket.Services.Payment.Model;

namespace EvenTicket.Services.Payment.Services;

public interface IExternalGatewayPaymentService
{
    Task<bool> PerformPayment(PaymentInfo paymentInfo);
}