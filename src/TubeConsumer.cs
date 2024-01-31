using ProGaudi.Tarantool.Client.Model;
using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public class FiFoTubeConsumer : TubeConsumer<FiFoTubeOptions>
    {
        public FiFoTubeConsumer(ITube queueTube) : base(queueTube) 
        {
            CheckTubeType(QueueType.fifo);
        }

        internal FiFoTubeConsumer(string tubeName, ClientOptions clientOptions)
            :base(tubeName, clientOptions)
        {
            CheckTubeType(QueueType.fifo);
        }
    }

    public class FiFoTtlTubeConsumer : TubeConsumer<FiFoTtlTubeOptions>
    {
        public FiFoTtlTubeConsumer(ITube queueTube) : base(queueTube) 
        {
            CheckTubeType(QueueType.fifottl);
        }

        internal FiFoTtlTubeConsumer(string tubeName, ClientOptions clientOptions)
            : base(tubeName, clientOptions)
        {
            CheckTubeType(QueueType.fifottl);
        }
    }

    public class LimFiFoTtlTubeConsumer : TubeConsumer<LimFiFoTtlTubeOptions>
    {
        public LimFiFoTtlTubeConsumer(ITube queueTube) : base(queueTube) 
        {
            CheckTubeType(QueueType.limfifottl);
        }
        internal LimFiFoTtlTubeConsumer(string tubeName, ClientOptions clientOptions)
            : base(tubeName, clientOptions)
        {
            CheckTubeType(QueueType.limfifottl);
        }
    }

    public class UTubeTubeConsumer : TubeConsumer<UTubeTubeOptions>
    {
        public UTubeTubeConsumer(ITube queueTube) : base(queueTube) 
        {
            CheckTubeType(QueueType.utube);
        }
        internal UTubeTubeConsumer(string tubeName, ClientOptions clientOptions)
            : base(tubeName, clientOptions)
        {
            CheckTubeType(QueueType.utube);
        }
    }

    public class UTubeTtlTubeConsumer : TubeConsumer<UTubeTtlTubeOptions>
    {
        public UTubeTtlTubeConsumer(ITube queueTube) : base(queueTube) 
        {
            CheckTubeType(QueueType.utubettl);
        }
        internal UTubeTtlTubeConsumer(string tubeName, ClientOptions clientOptions)
            : base(tubeName, clientOptions)
        {
            CheckTubeType(QueueType.utubettl);
        }
    }

    public class CustomTubeConsumer : TubeConsumer<AnyTubeOptions>
    {
        public CustomTubeConsumer(ITube queueTube) : base(queueTube)
        {
            CheckTubeType(QueueType.customtube);
        }
        internal CustomTubeConsumer(string tubeName, ClientOptions clientOptions)
            : base(tubeName, clientOptions)
        {
            CheckTubeType(QueueType.customtube);
        }

        protected override async Task<TubeTask?> Take(int? timeout, CancellationToken cancellationToken, AnyTubeOptions? opts = null)
            => await _queueTube.Take(timeout, opts!, cancellationToken);
    }

    public abstract class TubeConsumer<TQueueTubeOption> : TubeClient<TQueueTubeOption>, IConsumer<TQueueTubeOption>, IConsumerCommitter, IDisposable
        where TQueueTubeOption : TubeOptions
    {
        private readonly IQueue? _queue;
        private bool disposedValue;

        protected TubeConsumer(ITube queueTube): base(queueTube)
        {

        }

        protected TubeConsumer(string tubeName, ClientOptions clientOptions) : this(tubeName, Queue.GetQueue(clientOptions))
        {

        }

        TubeConsumer(string tubeName, IQueue queue) : this(queue.GetTube(tubeName).GetAwaiter().GetResult())
        {
            _queue = queue;
        }

        protected async virtual Task<TubeTask?> Take(int? timeout, CancellationToken cancellationToken, TQueueTubeOption? opts = null)
        {
            return await _queueTube.Take(timeout, cancellationToken);
        }

        public async Task<TubeTask> Release(TubeTask tubeTask, TQueueTubeOption? opts = null)
            => await _queueTube.Release(GetTaskId(tubeTask), opts ?? _defaultOptions);

        public async Task<ConsumeResult> Consume(int? timeout, CancellationToken cancellationToken, TQueueTubeOption? opts = null)
        {
            return new ConsumeResult(await Take(timeout, cancellationToken, opts), this);
        }

        public async Task<TubeTask> Delete(TubeTask tubeTask)
             => await _queueTube.Delete(GetTaskId(tubeTask));

        public async Task<TubeTask> Bury(TubeTask tubeTask)
             => await _queueTube.Bury(GetTaskId(tubeTask));

        public async Task<TubeTask> Touch(TubeTask tubeTask, TimeSpan delta)
             => await _queueTube.Touch(GetTaskId(tubeTask), delta);

        public async Task<TubeTask> Peek(TubeTask tubeTask)
            => await _queueTube.Peek(GetTaskId(tubeTask));

        async Task<TubeTask> IConsumerCommitter.Ack(TubeTask tubeTask)
            => await _queueTube.Ack(GetTaskId(tubeTask));

        private static ulong GetTaskId(TubeTask? tubeTask)
        {
            if (tubeTask?.TaskId == null)
                throw new ArgumentNullException(nameof(tubeTask));

            return tubeTask.TaskId.Value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _queue?.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
