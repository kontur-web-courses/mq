using MassTransit;
using Models;

namespace consumer;

public class MyConsumer: IConsumer<VeryImportantTaskModel>
{
    public Task Consume(ConsumeContext<VeryImportantTaskModel> context)
    {
        Console.WriteLine($"[{context.Message.Id}] {context.Message.VeryImportantTaskData}");
        return Task.CompletedTask;
    }
}