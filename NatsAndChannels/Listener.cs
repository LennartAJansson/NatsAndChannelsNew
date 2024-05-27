namespace NatsAndChannels;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NatsEvents;

internal class Listener(ILogger<Listener> logger, NatsChannel<CommandAction> channel)
  : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await foreach (CommandAction item in channel.Reader.ReadAllAsync(stoppingToken))
    {
      logger.LogInformation("Id: {id} Time: {time} Type: {type} Action: {action}",
        item.CorrelationId,item.TimeStamp,item.Type,item.Action);
    }
  }
}
