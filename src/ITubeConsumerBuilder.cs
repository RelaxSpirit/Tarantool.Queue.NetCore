using ProGaudi.Tarantool.Client.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public interface ITubeConsumerBuilder
    {
        Task<IConsumer<TQueueTubeOptions>> Build<TQueueTubeOptions>(string queueTubeName, bool newConnection = false)
            where TQueueTubeOptions : TubeOptions;

        Task<IConsumer<AnyTubeOptions>> BuildCustomTubeConsumer(string queueTubeName, bool newConnection = false);
    }
}
