using System.Diagnostics;
using Tarantool.Queues;
using Tarantool.Queues.Helpers;
using Tarantool.Queues.Options;

namespace TarantoolReader
{
    internal sealed class Consumer : IDisposable
    {
        private readonly AnyTubeOptions _options;
        private bool disposedValue;
        private Task _mainTask = null!;
        private readonly IConsumer<AnyTubeOptions> _consumer;
        int _taskCount = 0;
        TimeSpan _allTime = TimeSpan.Zero;
        readonly Stopwatch _sw = new ();

        internal Consumer(IConsumer<AnyTubeOptions> consumer, AnyTubeOptions options)
        {
            _options = options;
            _consumer = consumer;
        }

        internal Task StartConsume(CancellationToken cancellationToken)
        {
            _mainTask = Task.Run(async () =>
            {

                ulong lastId = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = await _consumer.Consume(null, cancellationToken, _options);

                        if (consumeResult.Task != null)
                        {
                            _taskCount++;
                            var writeDate = DateTime.Parse(consumeResult.Task!.TaskData).ToUniversalTime();
                            _allTime += consumeResult.ConsumeDate - writeDate;

                            _sw.Start();
                            await consumeResult.Commit();
                            _sw.Stop();

                            lastId = consumeResult.Task.TaskId!.Value;
                        }
                    }
                    catch (TaskCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading task from queue\n{ex}");
                    }

                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _mainTask?.Dispose();
                }
                disposedValue = true;
            }
        }

        public override string ToString()
        {
            var taskCount = Math.Max(_taskCount, 1);
            //return $"Utube {_options["utube"]}: Read - {_taskCount} average message queuing time - {_allTime / taskCount}, average task completion time {_sw.Elapsed / taskCount}";
            return $"Utube {_options["utube"]}: Read - {_taskCount} AMQT - {_allTime / taskCount}, average ACK time {(_sw.Elapsed / taskCount).TotalSeconds} sec.";
        }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
