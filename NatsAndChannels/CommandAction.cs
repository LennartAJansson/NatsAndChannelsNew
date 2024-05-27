namespace NatsAndChannels;

public class CommandAction : BaseEvent
{
  public required string Action { get; set; }
  public override string Type { get; init; } = "CommandAction";
}