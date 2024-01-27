namespace Tarantool.Queues.Model
{
    public partial class TubeTask
    {
        public ulong? TaskId { get; private set; } = null;
        public QueueTaskState? TaskState { get; private set; } = null;
        public string TaskData { get; private set; } = string.Empty;
    }
}
