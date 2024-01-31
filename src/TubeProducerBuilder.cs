using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public class TubeProducerBuilder : ITubeProducerBuilder
    {
        protected readonly IQueue _queue;
        public TubeProducerBuilder(IQueue queue)
        {
            _queue = queue;
        }

        public async Task<IProducer<TQueueTubeOption>> Build<TQueueTubeOption>(string queueTubeName)
            where TQueueTubeOption : TubeOptions
        {
            var tube = await _queue.GetTube(queueTubeName);
            return tube.TubeType switch
            {
                QueueType.fifo => (IProducer<TQueueTubeOption>) new FiFoTubeProducer(tube),
                QueueType.fifottl => (IProducer<TQueueTubeOption>)new FiFoTtlTubeProducer(tube),
                QueueType.limfifottl => (IProducer<TQueueTubeOption>)new LimFiFoTtlTubeProducer(tube),
                QueueType.utube => (IProducer<TQueueTubeOption>)new UTubeTubeProducer(tube),
                QueueType.utubettl => (IProducer<TQueueTubeOption>)new UTubeTtlTubeProducer(tube),
                _ => throw new NotSupportedException($"Tube type '{tube.TubeType}' is not a standard Tarantool Queue type"),
            };
        }

        public async Task<IProducer<TQueueTubeOption>> BuildCustomTubeProducer<TQueueTubeOption>(string queueTubeName)
            where TQueueTubeOption : AnyTubeOptions
        {
            var tube = await _queue.GetTube(queueTubeName);
            if (tube.TubeType != QueueType.customtube)
                throw new NotSupportedException($"Tube type '{tube.TubeType}' is standard Tarantool Queue type");

            return (IProducer<TQueueTubeOption>) new CustomTubeProducer(tube);
        }
    }
}
