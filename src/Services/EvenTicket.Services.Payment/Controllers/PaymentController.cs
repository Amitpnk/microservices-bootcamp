using Microsoft.AspNetCore.Mvc;

namespace EvenTicket.Services.Payment.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}