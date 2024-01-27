using Tarantool.Queues.Model;

namespace Tarantool.Queues
{
    public interface ITubeClient
    {
        Task<QueueTubeStatistic> GetStatistics();
    }
}
