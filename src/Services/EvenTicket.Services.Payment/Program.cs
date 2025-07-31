using EvenTicket.Infrastructure.MessagingBus;
using EvenTicket.Services.Payment.Services;
using EvenTicket.Services.Payment.Worker;
using Polly.Extensions.Http;
using Polly.CircuitBreaker;
using Polly;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register IHttpClientFactory (required for AddHttpClient)
builder.Services.AddHttpClient("ExternalGateway", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiConfigs:ExternalPaymentGateway:Uri"]);
})
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("ExternalGatewayRetryPolicy");
        var retryPolicy = GetRetryPolicy(logger);
        var circuitBreakerPolicy = GetCircuitBreakerPolicy(logger);

        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
    });

builder.Services.AddTransient<IExternalGatewayPaymentService, ExternalGatewayPaymentService>();

// Register IConfiguration (usually not needed, as it's registered by default)
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddHostedService<ServiceBusListener>();

builder.Services.AddSingleton<IMessageBus>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetValue<string>("ServiceBusConnectionString");
    var logger = provider.GetRequiredService<ILogger<AzServiceBusMessageBus>>();
    return new AzServiceBusMessageBus(logger, connectionString);
});

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddOpenApi();

Console.Title = "Payment";

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Swagger"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            3,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (outcome, timespan, retryAttempt, context) =>
            {
                logger.LogWarning(outcome.Exception,
                    $"Retry {retryAttempt} after {timespan.TotalSeconds}s " +
                    $"due to: {outcome.Exception?.Message ?? outcome.Result?.ReasonPhrase ?? "Unknown error"}");
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger)
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 2,
            durationOfBreak: TimeSpan.FromSeconds(15),
            onBreak: (outcome, breakDelay) =>
            {
                logger.LogWarning($"Circuit breaker opened for {breakDelay.TotalSeconds}s " +
                                  $"due to: {outcome.Exception?.Message ?? outcome.Result?.ReasonPhrase ?? "Unknown error"}"
                    );
            },
            onReset: () => logger.LogInformation("Circuit breaker reset."),
            onHalfOpen: () => logger.LogInformation("Circuit breaker half-open: next call is a trial.")
        );
}