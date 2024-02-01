using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Tarantool.Queues;
using Tarantool.Queues.Helpers;
using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace TarantoolReader
{
    public sealed class ConsumerService : IHostedService, IDisposable
    {
        private readonly ILogger<ConsumerService> _logger;
        private readonly ITubeConsumerBuilder _tubeConsumerBuilder;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task? _mainTask = null;
        public ConsumerService(ITubeConsumerBuilder tubeProducerBuilder, ILogger<ConsumerService> logger)
        {
            _tubeConsumerBuilder = tubeProducerBuilder;
            _logger = logger;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
            _mainTask?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumersOptionsArray = new AnyTubeOptions[]
                {
                    new(){ {"utube", "'HL:11'" } },
                    new(){ {"utube", "'HL:12'" } },
                    new(){ {"utube", "'HL:13'" } },
                    new(){ {"utube", "'HL:14'" } },
                    new(){ {"utube", "'HL:15'" } },
                    new(){ {"utube", "'HL:16'" } },
                    new(){ {"utube", "'HL:17'" } },
                    new(){ {"utube", "'HL:18'" } },
                    new(){ {"utube", "'HL:19'" } },
                    new(){ {"utube", "'HL:20'" } }
                };

            _mainTask = Task.Factory.StartNew(async() =>
            {
                //var consumer1 = await _tubeConsumerBuilder.Build<UTubeTtlTubeOptions>("queue_test_utubettl", true);
                //var consumer = await _tubeConsumerBuilder.Build<FiFoTubeOptions>("queue_test_fifo");

                //var consumer = await _tubeConsumerBuilder.BuildCustomTubeConsumer("queue_test_custom_tube", true);

                var consumers = consumersOptionsArray.Select(async c => new Consumer(await _tubeConsumerBuilder.BuildCustomTubeConsumer("queue_test_custom_tube", true), c))
                .Select(t => t.GetAwaiter().GetResult()).ToList();

                consumers.AsParallel().ForAll(async c => await c.StartConsume(_cancellationTokenSource.Token));

                Console.Clear();
                var cursorPosition = Console.GetCursorPosition();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top);
                        Console.Write(string.Join(Environment.NewLine, consumers.Select(c => c.ToString())));
                        await Task.Delay(500, _cancellationTokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error consume service");
                    }
                    
                }

            }, _cancellationTokenSource.Token);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource.Cancel();

            if(_mainTask != null)
                await _mainTask;
        }
    }
}
