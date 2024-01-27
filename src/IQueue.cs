using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tarantool.Queues.Queue;
using Tarantool.Queues.Options;
using Tarantool.Queues.Model;

namespace Tarantool.Queues
{
    public interface IQueue : IDisposable
    {
        Task<TubeCreationOptions> GetTubeCreationOptions(string tubeName);
        Task<string> GetTubeEngine(string tubeName);
        Task<int> GetTubeId(string tubeName);
        Task<Tube> GetTube(string queueName);
        Task<QueueState> GetState();
        Task<QueueStatistic> GetStatistics();

        Guid SessionUUid { get; }

        string Version { get; }
    }
}
