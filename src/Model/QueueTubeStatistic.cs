namespace Tarantool.Queues.Model
{
    public partial class QueueTubeStatistic
    {
        public Tasks TasksInfo { get; private set; } = null!;

        public Calls CallsInfo { get; private set; } = null!;

        public partial class Calls
        {
            public ulong Ttr { get; private set; }
            public ulong Touch { get; private set; }
            public ulong Bury { get; private set; }
            public ulong Put { get; private set; }
            public ulong Ack { get; private set; }
            public ulong Delay { get; private set; }
            public ulong Take { get; private set; }
            public ulong Kick { get; private set; }
            public ulong Release { get; private set; }
            public ulong Ttl { get; private set; }
            public ulong Delete { get; private set; }
        }

        public partial class Tasks
        {
            public ulong Taken { get; private set; }
            public ulong Done { get; private set; }
            public ulong Ready { get; private set; }
            public ulong Total { get; private set; }
            public ulong Delayed { get; private set; }
            public ulong Buried { get; private set; }
        }
    }
}
