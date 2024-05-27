namespace NatsEvents;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;

public class NatsReader<T>
  : ChannelReader<T> where T : class
{
  private readonly ILogger<NatsReader<T>> logger;


  public bool IsConnected { get; private set; }
  private readonly NatsConnection? connection;
  private INatsJSStream? jetStream;
  private readonly INatsJSContext? jetStreamContext;
  private INatsJSConsumer? consumer;
  private readonly NatsServiceConfig config;
  private readonly ILoggerFactory loggerFactory;


  public NatsReader(NatsServiceConfig config, ILoggerFactory loggerFactory)
  {
    this.config = config;
    this.loggerFactory = loggerFactory;
    logger = loggerFactory.CreateLogger<NatsReader<T>>();
    NatsOpts opts = NatsOpts.Default with
    {
      Url = $"nats://{config.Host}:{config.Port}",
      LoggerFactory = loggerFactory
    };
    connection = new NatsConnection(opts);
    jetStreamContext = new NatsJSContext(connection);
    this.config = config;
    jetStream = jetStreamContext!.CreateStreamAsync(new StreamConfig(name: config.Stream, subjects: config.Subjects)).Result;
    consumer = jetStreamContext.CreateOrUpdateConsumerAsync(config.Stream, new ConsumerConfig(config.Consumer)).Result;
  }

  public override bool CanCount { get; } = false;
  public override bool CanPeek { get; } = false;
  public override Task Completion { get; }
  public override int Count { get; }

  public override async IAsyncEnumerable<T> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    CancellationTokenSource cts = new();

    Console.CancelKeyPress += (_, e) =>
    {
      e.Cancel = true;
      cts.Cancel();
      //Completion.Wait(cts.Token);
    };

    //Read all items from the channel.
    await foreach (NatsJSMsg<T> jsMsg in consumer!.ConsumeAsync(serializer: new EventSerializer<T>(), cancellationToken: cts.Token))
    {
      if (jsMsg.Data is not null)
      {
        await jsMsg.AckAsync();
        yield return jsMsg.Data;
      }
    }
    await connection!.DisposeAsync();
  }

  public override ValueTask<T> ReadAsync(CancellationToken cancellationToken = default) =>
    //Read one item from the channel.
    throw new NotImplementedException();

  public override bool TryPeek([MaybeNullWhen(false)] out T item) =>
    //Peek at the next item in the channel.
    throw new NotImplementedException();

  public override bool TryRead([MaybeNullWhen(false)] out T item) =>
    //Read one item from the channel.
    throw new NotImplementedException();

  public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default) =>
    //True if an item is available in the channel; otherwise, false.
    throw new NotImplementedException();
}
