using External.PaymentGateway.Model;
using Microsoft.AspNetCore.Mvc;
using System;

namespace External.PaymentGateway.Controllers;

[ApiController]
[Route("api/paymentapprover")]
public class PaymentApproverController : ControllerBase
{
    [HttpPost]
    public IActionResult TryPayment([FromBody] PaymentDto payment)
    {

        int num = new Random().Next(1000);
        if (num > 500)
            return Ok(true);

        return Ok(false);
    }


    [HttpPost("error")]
    public IActionResult TryPayment2([FromBody] PaymentDto payment)
    {
        throw new InvalidOperationException("Internal error");
    }
}