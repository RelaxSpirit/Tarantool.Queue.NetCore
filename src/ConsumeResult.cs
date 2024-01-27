using Tarantool.Queues.Model;

namespace Tarantool.Queues
{
    public sealed class ConsumeResult
    {
        internal readonly IConsumerCommitter _consumer;
        private int _committed = 0;
        internal ConsumeResult(TubeTask? tubeTask, IConsumerCommitter consumer)
        {
            Task = tubeTask;
            ConsumeDate = DateTime.UtcNow;
            _consumer = consumer;
        }

        public TubeTask? Task { get; private set; }
        public DateTime ConsumeDate { get; private set; }
        public DateTime? CommitDate { get; private set; }
        internal async Task Commit()
        {
            if (Interlocked.Exchange(ref _committed, 1) == 0)
            {
                if (Task != null)
                {
                    Task = await _consumer.Ack(Task);
                    CommitDate = DateTime.UtcNow;
                }
            }
        }
    }
}
