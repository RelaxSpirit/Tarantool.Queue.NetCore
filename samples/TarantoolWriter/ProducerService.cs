using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Tarantool.Queues;
using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace TarantoolWriter
{
    public sealed class ProducerService : IHostedService, IDisposable
    {
        const int MAX_TASK_COUNT = 5000;
        private readonly ILogger<ProducerService> _logger;
        private readonly ITubeProducerBuilder _tubeProducerBuilder;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task? _mainTask = null;
        public ProducerService(ITubeProducerBuilder tubeProducerBuilder, ILogger<ProducerService> logger)
        {
            _tubeProducerBuilder = tubeProducerBuilder;
            _logger = logger;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
            _mainTask?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _mainTask = Task.Factory.StartNew(async () =>
            {
                //var producer = await _tubeProducerBuilder.Build<UTubeTtlTubeOptions>("queue_test_utubettl");
                var producer = await _tubeProducerBuilder.Build<FiFoTubeOptions>("queue_test_fifo");

                var produceOptionsArray = new UTubeTtlTubeOptions[]
                {
                    (UTubeTtlTubeOptions) TubeOptions.GetDefaultTubeOptions(QueueType.utubettl, "HL:11"),
                    (UTubeTtlTubeOptions)TubeOptions.GetDefaultTubeOptions(QueueType.utubettl, "HL:12"),
                    (UTubeTtlTubeOptions) TubeOptions.GetDefaultTubeOptions(QueueType.utubettl, "HL:13"),
                    (UTubeTtlTubeOptions)TubeOptions.GetDefaultTubeOptions(QueueType.utubettl, "HL:14"),
                    (UTubeTtlTubeOptions) TubeOptions.GetDefaultTubeOptions(QueueType.utubettl, "HL:15"),
                    (UTubeTtlTubeOptions)TubeOptions.GetDefaultTubeOptions(QueueType.utubettl, "HL:16"),
                    (UTubeTtlTubeOptions) TubeOptions.GetDefaultTubeOptions(QueueType.utubettl, "HL:17"),
                    (UTubeTtlTubeOptions)TubeOptions.GetDefaultTubeOptions(QueueType.utubettl, "HL:18"),
                    (UTubeTtlTubeOptions) TubeOptions.GetDefaultTubeOptions(QueueType.utubettl, "HL:19"),
                    (UTubeTtlTubeOptions)TubeOptions.GetDefaultTubeOptions(QueueType.utubettl, "HL:20")
                };

                var random = new Random(1);

                var maxOptionIndex = produceOptionsArray.Length - 1;
                var sw = new Stopwatch();

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    
                    try
                    {
                        int i = MAX_TASK_COUNT;
                        var cursorPosition = Console.GetCursorPosition();
                        while (i > 0)
                        {
                            //var opts = produceOptionsArray[random.Next(0, maxOptionIndex)];
                            sw.Start();
                            //var produceTask = await producer.Write($"{DateTime.Now}", opts);
                            var produceTask = await producer.Write($"{DateTime.Now}");
                            sw.Stop();
                            i--;
                            int writeCount = MAX_TASK_COUNT - i;
                            Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top);
                            Console.Write($"Recorded - {writeCount}, average write time {sw.Elapsed / writeCount}, total write time {sw.Elapsed}");
                        }

                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Task delivery to queue failed");
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
