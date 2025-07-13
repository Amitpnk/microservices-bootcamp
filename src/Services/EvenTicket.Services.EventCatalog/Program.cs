using EvenTicket.Services.EventCatalog.DbContexts;
using EvenTicket.Services.EventCatalog.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

services.AddDbContext<EventCatalogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<ICategoryRepository, CategoryRepository>();
services.AddScoped<IEventRepository, EventRepository>();

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddSwaggerGen();

services.AddControllers();
services.AddOpenApi();

Console.Title = "Event catalog";

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EventCatalogDbContext>();
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