using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public interface ITubeConsumerBuilder
    {
        Task<IConsumer<TQueueTubeOption>> Build<TQueueTubeOption>(string queueTubeName)
            where TQueueTubeOption : TubeOptions;

        Task<TConsumer?> BuildCustomTubeConsumer<TConsumer>(string queueTubeName)
            where TConsumer : TubeConsumer<AnyTubeOptions>;
    }
}
