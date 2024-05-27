namespace NatsAndChannels;

public class BaseEvent
{
  public Guid CorrelationId { get; init; } = Guid.NewGuid();
  public DateTimeOffset TimeStamp { get; init; } = DateTimeOffset.Now;
  public virtual string Type { get; init; } = string.Empty;
}