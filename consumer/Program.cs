using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace consumer;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var builder = CreateHostBuilder(args);

		var host = builder.Build();

		var busControl = host.Services.GetRequiredService<IBusControl>();

		try
		{
			Console.WriteLine("Starting consumer...");
			await busControl.StartAsync();
			Console.WriteLine("Consumer started. Press Enter to stop.");
			Console.ReadLine();
		}
		finally
		{
			Console.WriteLine("Stopping consumer...");
			await busControl.StopAsync();
			Console.WriteLine("Consumer stopped.");
		}
	}

	private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
		.ConfigureServices((hostContext, services) =>
		{
			services.AddMassTransit(config =>
			{
				config.AddConsumer<OrderExecutor>();

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
						e.ConfigureConsumer<OrderExecutor>(context);
					});
				});
			});
		});
}