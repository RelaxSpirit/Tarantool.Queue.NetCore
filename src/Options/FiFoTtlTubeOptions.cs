using System.Threading;
using Tarantool.Queues.Model;

namespace Tarantool.Queues.Options
{
    public class FiFoTtlTubeOptions : FiFoTubeOptions
    {
        private const string PRIORITY = "pri";
        internal const string TTL = "ttl";
        internal const string TTR = "ttr";
        internal const string DELAY = "delay";

        protected static readonly string[] s_fifoTtlOptions = 
        {
            PRIORITY,
            TTL,
            TTR,
            DELAY
        };

        public FiFoTtlTubeOptions() : base() { }

        /// <summary>
        /// Приоритет задачи (0 является наивысшим приоритетом и используется по умолчанию)
        /// </summary>
        public int? Priority
        {
            get
            {
                TryGetValue(PRIORITY, out string? value);
                return int.Parse(value ?? "0");
            }
            set
            {
                if (value.HasValue)
                    this[PRIORITY] = value.Value.ToString();
                else
                    Remove(PRIORITY);
            }
        }

        /// <summary>
        /// Время жизни задачи, поставленной в очередь. Если ttl не указано,
        /// то время устанавливается на бесконечность (если задача существует в очереди дольше ttl времени,
        /// она удаляется)
        /// </summary>
        public TimeSpan? Ttl
        {
            get
            {
                return GetTimeSpanValue(TTL);
            }
            set
            {
                SetTimeSpanValue(TTL, value);
            }
        }

        /// <summary>
        /// Время, отведенное consumer-у на работу над задачей. Если ttr не указано,
        /// то устанавливается то же время, что и ttl (если задача выполняется более ttr времени,
        /// ее статус меняется на «готово», чтобы ее мог взять на себя другой consumer)
        /// </summary>
        public TimeSpan? Ttr
        {
            get
            {
                return GetTimeSpanValue(TTR);
            }
            set
            {
                SetTimeSpanValue(TTR, value);
            }
        }

        /// <summary>
        /// Время ожидания перед началом выполнения задачи
        /// </summary>
        public TimeSpan? Delay
        {
            get
            {
                return GetTimeSpanValue(DELAY);
            }
            set
            {
                SetTimeSpanValue(DELAY, value);
            }
        }

        public override QueueType QueueType { get; protected set; } = QueueType.fifottl;

        protected override void ValidateOptionName(string optionName)
        {
            if (!s_fifoTtlOptions.Contains(optionName))
                ThrowValidateOptionNameException(optionName);
        }
    }
}
