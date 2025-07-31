using System;
using System.Runtime.InteropServices.ComTypes;
using EvenTicket.Infrastructure.MessagingBus;
using EvenTicket.Services.ShoppingBasket.DbContexts;
using EvenTicket.Services.ShoppingBasket.Repositories;
using EvenTicket.Services.ShoppingBasket.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

services.AddHttpClient("EventCatalog", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiConfigs:EventCatalog:Uri"]);
})
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("EventCatalogRetryPolicy");
        return GetRetryPolicy(logger);
    });

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddScoped<IBasketRepository, BasketRepository>();
services.AddScoped<IBasketLinesRepository, BasketLinesRepository>();
services.AddScoped<IEventRepository, EventRepository>();
//services.AddScoped<IMessageBus, AzServiceBusMessageBus>();

services.AddSingleton<IMessageBus>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<AzServiceBusMessageBus>>();
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetValue<string>("ServiceBusConnectionString");
    return new AzServiceBusMessageBus(logger, connectionString);
});

builder.Services.AddTransient<IEventCatalogService, EventCatalogService>();

services.AddDbContext<ShoppingBasketDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



services.AddSwaggerGen();
services.AddControllers();

services.AddOpenApi();

Console.Title = "Shopping Basket";

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingBasketDbContext>();
    dbContext.Database.Migrate();
}

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
