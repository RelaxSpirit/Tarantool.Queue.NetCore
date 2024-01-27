using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tarantool.Queues;
using TarantoolReader;

await new HostBuilder()
    .ConfigureAppConfiguration(option =>
    {
        option.AddJsonFile("appSettings.json");
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging(configure => configure.AddConsole());
        services.UseTarantoolQueue("192.168.1.116:3301");
        services.AddTubeConsumerBuilder();
        services.AddHostedService<ConsumerService>();
    })
    .RunConsoleAsync();
