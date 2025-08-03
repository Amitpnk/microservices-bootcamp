using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

//var appSettings = new AppSettings();
//builder.Configuration.Bind(appSettings);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.OnRejected = (context, token) =>
    {
        Console.WriteLine($"Request rejected for {context.HttpContext.Connection.RemoteIpAddress}");
        return ValueTask.CompletedTask;
    };

    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(60);
        options.PermitLimit = 5;
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();


app.MapGet("/", () => "YARP Reverse Proxy is running!");

app.UseHttpsRedirection();

app.UseRateLimiter();

app.MapReverseProxy()
    .RequireRateLimiting("fixed");


app.Run();
