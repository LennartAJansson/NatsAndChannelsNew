namespace NatsAndChannels;
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NatsEvents;

internal class Sender(ILogger<Sender> logger, NatsChannel<CommandAction> channel)
  : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      var time = DateTimeOffset.Now;
      string msg = $"Command sent at: {time}";
      logger.LogInformation("Command sent at: {time}", time);
      await channel.Writer.WriteAsync(new CommandAction { Action = msg }, stoppingToken);
      await Task.Delay(2000, stoppingToken);
    }
  }
}
