using static Tarantool.Queues.Queue;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public interface IAdminQueue : IDisposable
    {
        Task<Tube> CreateTube(string tubeName, TubeCreationOptions options);
        Task DeleteTube(string tubeName);
    }
}
