using MassTransit;
using consumer; // ← namespace, где лежит MyConsumer и MyMessage

var builder = Host.CreateApplicationBuilder(args);

// Регистрируем MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MyConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("my-queue", e =>
        {
            // e.UseInMemoryOutbox(context); // ← убрано, т.к. не требуется без транзакций БД
            e.ConfigureConsumer<MyConsumer>(context);
        });
    });
});

// Собираем и запускаем хост (без HTTP!)
var host = builder.Build();
await host.RunAsync();