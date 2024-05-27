namespace NatsEvents;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;

public class NatsWriter<T>
  : ChannelWriter<T> where T : class
{
  private readonly ILogger<NatsWriter<T>> logger;

  public bool IsConnected { get; private set; }
  private readonly NatsConnection? connection;
  private readonly INatsJSContext? jetStreamContext;
  private readonly NatsServiceConfig config;
  private readonly ILoggerFactory loggerFactory;

  public NatsWriter(NatsServiceConfig config, ILoggerFactory loggerFactory)
  {
    this.config = config;
    this.loggerFactory = loggerFactory;
    logger = this.loggerFactory.CreateLogger<NatsWriter<T>>();
    NatsOpts opts = NatsOpts.Default with
    {
      Url = $"nats://{config.Host}:{config.Port}",
      LoggerFactory = loggerFactory
    };
    connection = new NatsConnection(opts);
    jetStreamContext = new NatsJSContext(connection);
    _ = jetStreamContext!.CreateStreamAsync(new StreamConfig(name: config.Stream, subjects: config.Subjects)).Result;
  }

  public override bool TryComplete(Exception? error = null)
    => base.TryComplete(error);

  public override bool TryWrite(T item)
    => throw new NotImplementedException();

  public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default)
    //True if space is available in the channel; otherwise, false.
    => throw new NotImplementedException();

  public override async ValueTask WriteAsync(T item, CancellationToken cancellationToken = default)
  {
    //Write the item to the channel.

    CancellationTokenSource cts = new();

    Console.CancelKeyPress += (_, e) =>
    {
      e.Cancel = true;
      cts.Cancel();
    };
    PubAckResponse ack = await jetStreamContext!.PublishAsync(subject: config.Subject, data: item, serializer: new EventSerializer<T>(), cancellationToken: cts.Token);
    ack.EnsureSuccess();
  }
}


