using Tarantool.Queues.Model;

namespace Tarantool.Queues.Helpers
{
    public static class ConsumeResultHelper
    {
        public static async Task Commit(this ConsumeResult consumeResult)
        {
            await consumeResult.Commit();
        }
    }
}
