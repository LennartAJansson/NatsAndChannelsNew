namespace NatsEvents;

using System.Threading.Channels;

public class NatsChannel<T>(NatsReader<T> reader, NatsWriter<T> writer)
  : Channel<NatsWriter<T>, NatsReader<T>> where T : class
{
  public new NatsReader<T> Reader { get; } = reader;
  public new NatsWriter<T> Writer { get; } = writer;
}
