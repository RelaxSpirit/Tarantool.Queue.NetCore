using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public class FiFoTubeProducer : TubeProducer<FiFoTubeOptions>
    {
        public FiFoTubeProducer(ITube queueTube) : base(queueTube) 
        {
            CheckTubeType(QueueType.fifo);
        }
    }

    public class FiFoTtlTubeProducer : TubeProducer<FiFoTtlTubeOptions>
    {
        public FiFoTtlTubeProducer(ITube queueTube) : base(queueTube) 
        {
            CheckTubeType(QueueType.fifottl);
        }
    }

    public class LimFiFoTtlTubeProducer : TubeProducer<LimFiFoTtlTubeOptions>
    {
        public LimFiFoTtlTubeProducer(ITube queueTube) : base(queueTube) 
        {
            CheckTubeType(QueueType.limfifottl);
        }
    }

    public class UTubeTubeProducer : TubeProducer<UTubeTubeOptions>
    {
        public UTubeTubeProducer(ITube queueTube) : base(queueTube) 
        {
            CheckTubeType(QueueType.utube);
        }
    }

    public class UTubeTtlTubeProducer : TubeProducer<UTubeTtlTubeOptions>
    {
        public UTubeTtlTubeProducer(ITube queueTube) : base(queueTube) 
        {
            CheckTubeType(QueueType.utubettl);
        }
    }
    public abstract class TubeProducer<TQueueTubeOption> : TubeClient<TQueueTubeOption>, IProducer<TQueueTubeOption>
        where TQueueTubeOption : TubeOptions
    {
        protected TubeProducer(ITube queueTube) : base(queueTube)
        {

        }

        public async Task<ulong> Kick(ulong count)
            => await _queueTube.Kick(count);

        public async Task ReleaseAllTask()
            => await _queueTube.ReleaseAll();

        public async Task<TubeTask> Write(string data, TQueueTubeOption? opts = null)
            => await _queueTube.Put(data, opts ?? _defaultOptions);
    }
}
