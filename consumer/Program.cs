using MassTransit;

namespace consumer;

public static class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		
		builder.Services.AddMassTransit(config =>
		{
			config.UsingRabbitMq((context, cfg) =>
			{
				cfg.Host("localhost", "/", h =>
				{
					h.Username("guest");
					h.Password("guest");
				});

				cfg.ReceiveEndpoint("my-queue", e =>
				{
					e.UseInMemoryOutbox(context);
					e.Consumer<OrderExecutor>();
				});
			});
		});

		var app = builder.Build();

		app.Run();
	}
}