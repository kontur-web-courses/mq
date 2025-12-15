using MassTransit;
using MassTransit.Testing;
using Models;
using System.Text.Json;

namespace Tests;

public class ProducerConsumerTests
{
    private const string LogPath = @"c:\Users\angel\RiderProjects\mq\.cursor\debug.log";
    private const string SessionId = "debug-session";
    private const string RunId = "run1";

    private static void Log(string hypothesisId, string location, string message, object data)
    {
        // #region agent log
        var payload = new
        {
            sessionId = SessionId,
            runId = RunId,
            hypothesisId,
            location,
            message,
            data,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        var line = JsonSerializer.Serialize(payload);
        try
        {
            File.AppendAllText(LogPath, line + Environment.NewLine);
        }
        catch
        {
            // ignore logging errors
        }
        // #endregion
    }

    [Fact]
    public async Task Producer_Should_Send_Message_To_Queue()
    {
        // Arrange
        var harness = new InMemoryTestHarness();
        harness.OnConfigureInMemoryBus += cfg =>
        {
            cfg.ReceiveEndpoint(QueueConstants.DefaultQueueName, e =>
            {
                e.Consumer<MyConsumer>();
            });
        };

        await harness.Start();

        try
        {
            Log("H1", "Producer_Should_Send_Message_To_Queue", "started", new { harness.InputQueueAddress });
            var sendEndpoint = await harness.GetSendEndpoint(new Uri($"queue:{QueueConstants.DefaultQueueName}"));
            var message = new MyMessage 
            { 
                MessageId = Guid.NewGuid(), 
                Content = "Test message" 
            };

            // Act
            Log("H1", "Producer_Should_Send_Message_To_Queue", "before send", new { message.MessageId, message.Content });
            await sendEndpoint.Send(message);

            // Assert
            Assert.True(await harness.Consumed.Any<MyMessage>());
            var consumed = harness.Consumed.Select<MyMessage>().First();
            Log("H1", "Producer_Should_Send_Message_To_Queue", "consumed", new { consumed.Context.Message.MessageId, consumed.Context.Message.Content });
            Assert.Equal(message.MessageId, consumed.Context.Message.MessageId);
            Assert.Equal(message.Content, consumed.Context.Message.Content);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Consumer_Should_Receive_Message_From_Queue()
    {
        // Arrange
        var harness = new InMemoryTestHarness();
        harness.Consumer<MyConsumer>();

        await harness.Start();

        try
        {
            var message = new MyMessage 
            { 
                MessageId = Guid.NewGuid(), 
                Content = "Test consumer message" 
            };

            // Act
            Log("H2", "Consumer_Should_Receive_Message_From_Queue", "before send", new { message.MessageId, message.Content });
            await harness.InputQueueSendEndpoint.Send(message);

            // Assert
            Assert.True(await harness.Consumed.Any<MyMessage>());
            var consumed = harness.Consumed.Select<MyMessage>().First();
            Log("H2", "Consumer_Should_Receive_Message_From_Queue", "consumed", new { consumed.Context.Message.MessageId, consumed.Context.Message.Content });
            Assert.Equal(message.MessageId, consumed.Context.Message.MessageId);
            Assert.Equal(message.Content, consumed.Context.Message.Content);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Publisher_Should_Send_Message()
    {
        // Arrange
        var harness = new InMemoryTestHarness();
        harness.OnConfigureInMemoryBus += cfg =>
        {
            cfg.ReceiveEndpoint(QueueConstants.DefaultQueueName, e =>
            {
                e.Consumer<MyConsumer>();
            });
        };

        await harness.Start();

        try
        {
            var sendEndpointProvider = harness.Bus;
            var publisher = new MyPublisher(sendEndpointProvider, QueueConstants.DefaultQueueName);
            var testContent = "Publisher test message";

            // Act
            Log("H3", "Publisher_Should_Send_Message", "before publish", new { testContent });
            await publisher.Publish(testContent);

            // Assert
            Assert.True(await harness.Consumed.Any<MyMessage>());
            var consumed = harness.Consumed.Select<MyMessage>().First();
            Log("H3", "Publisher_Should_Send_Message", "consumed", new { consumed.Context.Message.Content });
            Assert.Equal(testContent, consumed.Context.Message.Content);
        }
        finally
        {
            await harness.Stop();
        }
    }
}

