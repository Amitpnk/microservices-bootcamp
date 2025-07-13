using EvenTicket.Infrastructure.MessagingBus;
using EvenTicket.Services.Ordering.DbContexts;
using EvenTicket.Services.Ordering.Extensions;
using EvenTicket.Services.Ordering.Messaging;
using EvenTicket.Services.Ordering.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

services.AddScoped<IOrderRepository, OrderRepository>();

//Specific DbContext for use from singleton AzServiceBusConsumer
var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

//services.Configure<ServiceBusSettings>(builder.Configuration.GetSection(ServiceBusSettings.SectionName));

services.AddSingleton(new OrderRepository(optionsBuilder.Options));

services.AddSingleton<IMessageBus, AzServiceBusMessageBus>();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering API", Version = "v1" });
});

services.AddSingleton<IAzServiceBusConsumer, AzServiceBusConsumer>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

Console.Title = "Ordering";
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
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
app.UseAzServiceBusConsumer();
app.Run();