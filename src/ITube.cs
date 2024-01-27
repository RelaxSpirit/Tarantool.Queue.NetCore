using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public interface ITube
    {
        string Name { get; }
        TubeCreationOptions CreationOptions { get;}
        QueueType TubeType { get; }
        Task<QueueTubeStatistic> GetStatistics();
        Task<TubeTask> Put(string data, TubeOptions opts);
        Task<TubeTask?> Take(int? timeout, CancellationToken cancellationToken);
        Task<TubeTask> Ack(ulong taskId);
        Task<TubeTask> Delete(ulong taskId);
        Task<TubeTask> Release(ulong task_id, TubeOptions opts);
        Task<TubeTask> Bury(ulong taskId);
        Task<ulong> Kick(ulong count);
        Task<TubeTask> Peek(ulong taskId);
        Task<TubeTask> Touch(ulong taskId, TimeSpan delta);
        Task ReleaseAll();
    }
}
