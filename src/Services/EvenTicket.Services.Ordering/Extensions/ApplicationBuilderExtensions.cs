using EvenTicket.Services.Ordering.Messaging;

namespace EvenTicket.Services.Ordering.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IAzServiceBusConsumer Consumer { get; set; }

    public static IApplicationBuilder UseAzServiceBusConsumer(this IApplicationBuilder app)
    {
        Consumer = app.ApplicationServices.GetService<IAzServiceBusConsumer>();
        var life = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        life.ApplicationStarted.Register(OnStarted);
        life.ApplicationStopping.Register(OnStopping);

        return app;
    }

    private static void OnStarted()
    {
        Consumer.Start();
    }

    private static void OnStopping()
    {
        Consumer.Stop();
    }
}