using Tarantool.Queues.Model;

namespace Tarantool.Queues.Options
{
    public partial class TubeCreationOptions : TubeOptions
    {
        internal const string TEMPORARY = "temporary";
        internal const string IF_NOT_EXISTS = "if_not_exists";
        internal const string ON_TASK_CHANGE = "on_task_change";
        internal const string CAPACITY = "capacity";
        protected static readonly string[] _fifoOptions = new string[]
        {
            TEMPORARY,
            IF_NOT_EXISTS,
            ON_TASK_CHANGE,
            CAPACITY
        };

        TubeCreationOptions(QueueType queueType)
        {
            QueueType = queueType;
        }

        public TubeCreationOptions()
        {
            QueueType = QueueType.customtube;
        }

        public override QueueType QueueType { get; protected set; }

        internal void SetQueueType(QueueType queueType)
        {
            QueueType = queueType;
        }

        /// <summary>
        /// Если true, содержимое очереди не сохраняется на диске
        /// </summary>
        public bool Temporary
        {
            get
            {
                TryGetValue(TEMPORARY, out string? value);
                return bool.Parse(value ?? "false");
            }
            set
            {
                this[TEMPORARY] = value ? "true" : "false";
            }
        }

        /// <summary>
        /// Если true, ошибка не будет возвращена, если tube уже существует
        /// </summary>
        public bool IfNotExists
        {
            get
            {
               TryGetValue(IF_NOT_EXISTS, out string? value);
                return bool.Parse(value ?? "false");
            }
            set
            {
                this[IF_NOT_EXISTS] = value ? "true" : "false";
            }
        }

        /// <summary>
        /// Ограничитель размера очереди
        /// </summary>
        public int? Capacity
        {
            get
            {
                TryGetValue(CAPACITY, out string? value);
                return value is null ? null : int.Parse(value);
            }
            set
            {
                if (value.HasValue)
                    this[CAPACITY] = value.Value.ToString();
                else
                    Remove(CAPACITY);
            }
        }

        protected override void ValidateOptionName(string optionName)
        {
            if((optionName == CAPACITY && QueueType != QueueType.limfifottl) || !_fifoOptions.Contains(optionName))
                ThrowValidateCreationOptionNameException(optionName);
        }

        public static TubeCreationOptions GetDefaultTubeCreationOptions(QueueType queueType)
        {
            return new TubeCreationOptions(queueType);
        }
    }
}
