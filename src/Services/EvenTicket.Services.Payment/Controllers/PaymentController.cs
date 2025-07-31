using EvenTicket.Services.Payment.Model;
using EvenTicket.Services.Payment.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvenTicket.Services.Payment.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly IExternalGatewayPaymentService _externalGatewayPaymentService;

    public PaymentController(IExternalGatewayPaymentService externalGatewayPaymentService)
    {
        _externalGatewayPaymentService = externalGatewayPaymentService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var paymentInfo = new PaymentInfo();
        //{
        //    Total = 100,  
        //    CardNumber = "1111-1111-1111-1111",  
        //    CardName = "Amit Naik",  
        //    CardExpiration = "12/30"  
        //};



        var result = await _externalGatewayPaymentService.PerformPayment(paymentInfo);
        return Ok(result);
    }
}