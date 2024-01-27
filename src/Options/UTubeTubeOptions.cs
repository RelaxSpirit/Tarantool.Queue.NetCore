using Tarantool.Queues.Model;

namespace Tarantool.Queues.Options
{
    public class UTubeTubeOptions : TubeOptions
    {
        internal const string UTUBE = "utube";

        public UTubeTubeOptions(string? uTubeName)
        {
            UTubeName = uTubeName ?? string.Empty;
        }

        public override QueueType QueueType { get; protected set; } = QueueType.utube;

        /// <summary>
        /// Имя подочереди. Подочереди разделяют поток задач в соответствии с именем подочереди:
        /// невозможно одновременно извлечь две задачи из подочереди, каждая подочередь выполняется
        /// в строгом порядке FIFO, по одной задаче за раз.
        /// </summary>
        public string UTubeName
        {
            get
            {
                return this[UTUBE].Replace("'", string.Empty);
            }
            set
            {
                this[UTUBE] = $"'{value}'";
            }
        }

        protected override void ValidateOptionName(string optionName)
        {
            if(optionName != UTUBE)
                ThrowValidateOptionNameException(optionName);
        }
    }
}
