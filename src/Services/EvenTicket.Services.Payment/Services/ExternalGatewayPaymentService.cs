using System.Net.Http.Headers;
using System.Text.Json;
using EvenTicket.Services.Payment.Model;
using Polly.CircuitBreaker;

namespace EvenTicket.Services.Payment.Services;

public class ExternalGatewayPaymentService : IExternalGatewayPaymentService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ExternalGatewayPaymentService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }


    public async Task<bool> PerformPayment(PaymentInfo paymentInfo)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ExternalGateway");
            var dataAsString = JsonSerializer.Serialize(paymentInfo);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(
                _configuration.GetValue<string>("ApiConfigs:ExternalPaymentGateway:Uri") + "/api/paymentapprover",
                content);

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<bool>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (BrokenCircuitException ex)
        {
            // Handle circuit breaker open state
            throw new ApplicationException("Payment service is temporarily unavailable due to repeated errors. Please try again later.", ex);
        }
    }
}