namespace NatsEvents;

using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class NatsExtensions
{
  public static IServiceCollection AddNatsChannel<T>(this IServiceCollection services, Func<NatsServiceConfig> configFunc)
    where T : class
  {
    var config = configFunc.Invoke();
    _ = services
      //.AddNats(5,ConfigureOpts,ConfigureConnection,null)
      .AddSingleton<NatsWriter<T>>(sp=>new NatsWriter<T>(config, sp.GetRequiredService<ILoggerFactory>()))
      .AddSingleton<NatsReader<T>>(sp => new NatsReader<T>(config, sp.GetRequiredService<ILoggerFactory>()))
      .AddSingleton<NatsChannel<T>>();
    return services;
  }
}




//NatsOpts ConfigureOpts(NatsOpts opts)
//{
//  throw new NotImplementedException();
//}
//void ConfigureConnection(NatsConnection connection)
//{
//  throw new NotImplementedException();
//}

