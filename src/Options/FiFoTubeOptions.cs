using Tarantool.Queues.Model;

namespace Tarantool.Queues.Options
{
    public class FiFoTubeOptions : TubeOptions
    {
        public override QueueType QueueType { get; protected set; } = QueueType.fifo;

        public FiFoTubeOptions() { }

        protected override void ValidateOptionName(string optionName)
        {
            ThrowValidateOptionNameException(optionName);
        }
    }
}
