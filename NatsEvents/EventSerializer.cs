namespace NatsEvents;

using System.Buffers;
using System.Text.Json;

using NATS.Client.Core;

public class EventSerializer<T> : INatsSerializer<T>
{
  public T? Deserialize(in ReadOnlySequence<byte> buffer)
  {
    byte[] buf = buffer.ToArray();
    T? action = JsonSerializer.Deserialize<T>(buf);
    return action;
  }

  public void Serialize(IBufferWriter<byte> bufferWriter, T value)
  {
    byte[] buf = JsonSerializer.SerializeToUtf8Bytes(value);
    bufferWriter.Write(buf);
  }
}


