using MassTransit.Testing;
using Models;

namespace consumer;

public class MyConsumerTest
{
    [Test]
    public async Task MsgConsumeTest()
    {
        var harness = new InMemoryTestHarness();
        var consumerHarness = harness.Consumer<MyConsumer>();

        await harness.Start();

        await harness.InputQueueSendEndpoint.Send(new Class1 { message = "test msg"});
        await harness.InputQueueSendEndpoint.Send(new Class1 { message = "test msg2"});
        
        var message = harness.Consumed.Select<Class1>().First().MessageObject as Class1;
        var consumedMessage = consumerHarness.Consumed.Select<Class1>().FirstOrDefault().MessageObject as Class1;
        
        Assert.That(message!.message, Is.EqualTo("test msg"));
        Assert.That(consumedMessage!.message, Is.EqualTo("test msg"));
        
        message = harness.Consumed.Select<Class1>().Skip(1).First().MessageObject as Class1;
        consumedMessage = consumerHarness.Consumed.Select<Class1>().Skip(1).First().MessageObject as Class1;
        
        Assert.That(message!.message, Is.EqualTo("test msg2"));
        Assert.That(consumedMessage!.message, Is.EqualTo("test msg2"));
    }
}