using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Tarantool.Queues;
using Tarantool.Queues.Helpers;
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
            _mainTask = Task.Factory.StartNew(async () =>
            {
                //var consumer = await _tubeConsumerBuilder.Build<UTubeTtlTubeOptions>("queue_test_utubettl");
                var consumer = await _tubeConsumerBuilder.Build<FiFoTubeOptions>("queue_test_fifo");

                var cursorPosition = Console.GetCursorPosition();
                int taskCount = 0;
                TimeSpan allTime = TimeSpan.Zero;
                var sw = new Stopwatch();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = await consumer.Consume(null, _cancellationTokenSource.Token);

                        if (consumeResult.Task != null)
                        {
                            taskCount++;
                            var writeDate = DateTime.Parse(consumeResult.Task!.TaskData).ToUniversalTime();
                            allTime += consumeResult.ConsumeDate - writeDate;
                            
                            sw.Start();
                            await consumeResult.Commit();
                            sw.Stop();

                            Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top);
                            Console.Write($"Read - {taskCount} average message queuing time - {allTime / taskCount}, average task completion time {sw.Elapsed / taskCount} ");
                        }
                    }
                    catch(TaskCanceledException)
                    {

                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Error reading task from queue") ;
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
