using MassTransit;
using MassTransit.Logging;
using Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(options =>
{
    options.Host(new Uri($"amqps://guest:guest@localhost:5672/"));
    
    options.ReceiveEndpoint(nameof(TextMessage), x =>
    {
        x.ConfigureConsumeTopology = false;
        x.SetQueueArgument("x-consumer-timeout", 10000);
        
        x.Consumer<MyHighTextMessageConsumer>(c =>
        {
            c.Options<BatchOptions>(batchOptions =>
            {
                batchOptions.TimeLimit = TimeSpan.FromSeconds(120);
                batchOptions.MessageLimit = 10;
            });
        });
    });
    
    LogContext.ConfigureCurrentLogContext(new TextWriterLoggerFactory(Console.Out, 
        new OptionsWrapper<TextWriterLoggerOptions>(new TextWriterLoggerOptions()
        {
            LogLevel = LogLevel.Debug
        })));
});
bus.Start();
Console.WriteLine("Listening for messages. Hit <return> to quit.");
Console.ReadLine();
bus.Stop();


class MyHighTextMessageConsumer : IConsumer<Batch<TextMessage>>
{
    public async Task Consume(ConsumeContext<Batch<TextMessage>> context)
    {
        Console.WriteLine("Processing batch of {0} messages", context.Message.Length);
        foreach (var message in context.Message)
        {
            Console.WriteLine("Got message: {0} {1} {2}", DateTime.UtcNow, message.Message.Text, message.Message.Priority);
        }
    }
}