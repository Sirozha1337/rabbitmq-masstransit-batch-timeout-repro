using MassTransit;
using MassTransit.Logging;
using Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(options =>
{
    options.Host(new Uri($"amqps://guest:guest@localhost:5672/"));
    
    LogContext.ConfigureCurrentLogContext(new TextWriterLoggerFactory(Console.Out, 
        new OptionsWrapper<TextWriterLoggerOptions>(new TextWriterLoggerOptions()
    {
        LogLevel = LogLevel.Debug
    })));
});

bus.Start();

Console.WriteLine("Publishing first message");
var endpoint = await bus.GetSendEndpoint(new Uri("queue:TextMessage"));
await endpoint.SendBatch( new TextMessage[]
{
    new() { Text = $"High Priority 1", Priority = "High" },
    new() { Text = $"High Priority 2", Priority = "High" },
    new() { Text = $"High Priority 3", Priority = "High" }
});
await endpoint.SendBatch( new TextMessage[]
{
    new() { Text = $"High Priority 4", Priority = "High" },
    new() { Text = $"High Priority 5", Priority = "High" },
    new() { Text = $"High Priority 6", Priority = "High" }
});
Console.WriteLine("Published first message");

bus.Stop();

