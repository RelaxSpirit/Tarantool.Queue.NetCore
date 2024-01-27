using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public interface IConsumer<TQueueTubeOption> : ITubeClient
        where TQueueTubeOption : TubeOptions
    {
        Task<TubeTask> Release(TubeTask tubeTask, TQueueTubeOption? opts = null);
        Task<ConsumeResult> Consume(int? timeout, CancellationToken cancellationToken, TQueueTubeOption? opts = null);
        Task<TubeTask> Delete(TubeTask tubeTask);
        Task<TubeTask> Bury(TubeTask tubeTask);
        Task<TubeTask> Touch(TubeTask tubeTask, TimeSpan delta);
        Task<TubeTask> Peek(TubeTask tubeTask);
    }

    internal interface IConsumerCommitter
    {
        Task<TubeTask> Ack(TubeTask tubeTask);
    }
}
