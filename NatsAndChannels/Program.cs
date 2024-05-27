using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NatsAndChannels;

using NatsEvents;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

builder.Services
  .AddNatsChannel<CommandAction>(() => builder.Configuration
    .GetSection("NatsServiceConfig").Get<NatsServiceConfig>()
    ?? throw new ArgumentException("NatsServiceConfig section is missing in config"))
  //Add further contracts

  .AddHostedService<Sender>()
  .AddHostedService<Listener>();

await builder.Build().RunAsync();
