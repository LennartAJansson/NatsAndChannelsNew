namespace NatsEvents;
public sealed class NatsServiceConfig
{
  public required string Host { get; set; }
  public int Port { get; set; }
  public required string Stream { get; set; }
  public required string[] Subjects { get; set; }
  public required string Subject { get; set; }
  public required string Consumer { get; set; }
}
