using EvenTicket.Services.ShoppingBasket.DbContexts;
using EvenTicket.Services.ShoppingBasket.Repositories;
using EvenTicket.Services.ShoppingBasket.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddScoped<IBasketRepository, BasketRepository>();
services.AddScoped<IBasketLinesRepository, BasketLinesRepository>();
services.AddScoped<IEventRepository, EventRepository>();
//todo: adding eventcatalog url
services.AddHttpClient<IEventCatalogService, EventCatalogService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiConfigs:EventCatalog:Uri"]));

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