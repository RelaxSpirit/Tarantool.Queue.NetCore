using ProGaudi.Tarantool.Client.Model;
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

        public async Task<IConsumer<TQueueTubeOptions>> Build<TQueueTubeOptions>(string queueTubeName, bool newConnection = false)
            where TQueueTubeOptions : TubeOptions
        {
            if (!newConnection)
            {
                var tube = await _queue.GetTube(queueTubeName);

                return tube.TubeType switch
                {
                    QueueType.fifo => (IConsumer<TQueueTubeOptions>)new FiFoTubeConsumer(tube),
                    QueueType.fifottl => (IConsumer<TQueueTubeOptions>)new FiFoTtlTubeConsumer(tube),
                    QueueType.limfifottl => (IConsumer<TQueueTubeOptions>)new LimFiFoTtlTubeConsumer(tube),
                    QueueType.utube => (IConsumer<TQueueTubeOptions>)new UTubeTubeConsumer(tube),
                    QueueType.utubettl => (IConsumer<TQueueTubeOptions>)new UTubeTtlTubeConsumer(tube),
                    _ => throw new NotSupportedException($"Tube type '{tube.TubeType}' is not a standard Tarantool Queue type"),
                };
            }
            else
            {
                var optionsType = typeof(TQueueTubeOptions);
                if (optionsType == typeof(FiFoTubeOptions))
                    return (IConsumer<TQueueTubeOptions>) new FiFoTubeConsumer(queueTubeName, _queue.ClientOptions);
                if (optionsType == typeof(FiFoTtlTubeOptions))
                    return (IConsumer<TQueueTubeOptions>)new FiFoTtlTubeConsumer(queueTubeName, _queue.ClientOptions);
                if (optionsType == typeof(LimFiFoTtlTubeOptions))
                    return (IConsumer<TQueueTubeOptions>)new LimFiFoTtlTubeConsumer(queueTubeName, _queue.ClientOptions);
                if (optionsType == typeof(UTubeTubeOptions))
                    return (IConsumer<TQueueTubeOptions>)new UTubeTubeConsumer(queueTubeName, _queue.ClientOptions);
                if (optionsType == typeof(UTubeTtlTubeOptions))
                    return (IConsumer<TQueueTubeOptions>)new UTubeTtlTubeConsumer(queueTubeName, _queue.ClientOptions);

                throw new NotSupportedException($"Tube options type '{optionsType}' is not type a standard Tarantool Queue");
            }
        }

        public async Task<IConsumer<AnyTubeOptions>> BuildCustomTubeConsumer(string queueTubeName, bool newConnection = false)
        {
            if (!newConnection)
            {
                var tube = await _queue.GetTube(queueTubeName);

                if (tube.TubeType != QueueType.customtube)
                    throw new NotSupportedException($"Tube type '{tube.TubeType}' is standard Tarantool Queue type");

                return new CustomTubeConsumer(tube);
            }
            else
            {
                return new CustomTubeConsumer(queueTubeName, _queue.ClientOptions);
            }
        }

    }
}
