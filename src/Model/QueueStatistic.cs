namespace Tarantool.Queues.Model
{
    public partial class QueueStatistic
    {
        public Dictionary<string, QueueTubeStatistic> QueueTubesStatistic { get; } = new ();
    }
}
