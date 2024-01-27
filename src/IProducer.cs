using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public interface IProducer<TTubeOptions> : ITubeClient
        where TTubeOptions : TubeOptions
    {
        Task<TubeTask> Write(string data, TTubeOptions? opts = null);
        Task ReleaseAllTask();
        Task<ulong> Kick(ulong count);
        
    }
}
