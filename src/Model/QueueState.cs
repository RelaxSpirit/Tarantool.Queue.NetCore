namespace Tarantool.Queues.Model
{
    public enum QueueState
    {
        INIT = 'i',
        STARTUP = 's',
        RUNNING = 'r',
        ENDING = 'e',
        WAITING = 'w',
    }
}
