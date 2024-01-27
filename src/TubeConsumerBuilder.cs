using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public class TubeConsumerBuilder : ITubeConsumerBuilder
    {
        protected readonly IQueue _queue;
        public TubeConsumerBuilder(IQueue queue)
        {
            _queue = queue;
        }

        public async Task<IConsumer<TQueueTubeOption>> Build<TQueueTubeOption>(string queueTubeName)
            where TQueueTubeOption : TubeOptions
        {
            var tube = await _queue.GetTube(queueTubeName);
            
            return tube.TubeType switch
            {
                QueueType.fifo => (IConsumer<TQueueTubeOption>) new FiFoTubeConsumer(tube),
                QueueType.fifottl => (IConsumer<TQueueTubeOption>)new FiFoTtlTubeConsumer(tube),
                QueueType.limfifottl => (IConsumer<TQueueTubeOption>)new LimFiFoTtlTubeConsumer(tube),
                QueueType.utube => (IConsumer<TQueueTubeOption>)new UTubeTubeConsumer(tube),
                QueueType.utubettl => (IConsumer<TQueueTubeOption>)new UTubeTtlTubeConsumer(tube),
                _ => throw new NotSupportedException(),
            };
        }

        public async Task<TConsumer?> BuildCustomTubeConsumer<TConsumer>(string queueTubeName)
            where TConsumer : TubeConsumer<AnyTubeOptions>
        {
            var tube = await _queue.GetTube(queueTubeName);
            return Activator.CreateInstance(typeof(TConsumer), tube) as TConsumer;
        }
    }
}
