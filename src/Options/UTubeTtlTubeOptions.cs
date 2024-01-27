using Tarantool.Queues.Model;

namespace Tarantool.Queues.Options
{
    public class UTubeTtlTubeOptions : UTubeTubeOptions
    {
        static readonly string[] s_utubeTtlOptions =
        {
            UTUBE,
            FiFoTtlTubeOptions.TTL,
            FiFoTtlTubeOptions.TTR,
            FiFoTtlTubeOptions.DELAY
        };

        public UTubeTtlTubeOptions(string? uTubeName) : base(uTubeName)
        {
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
                return GetTimeSpanValue(FiFoTtlTubeOptions.TTL);
            }
            set
            {
                SetTimeSpanValue(FiFoTtlTubeOptions.TTL, value);
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
                return GetTimeSpanValue(FiFoTtlTubeOptions.TTR);
            }
            set
            {
                SetTimeSpanValue(FiFoTtlTubeOptions.TTR, value);
            }
        }

        /// <summary>
        /// Время ожидания перед началом выполнения задачи
        /// </summary>
        public TimeSpan? Delay
        {
            get
            {
                return GetTimeSpanValue(FiFoTtlTubeOptions.DELAY);
            }
            set
            {
                SetTimeSpanValue(FiFoTtlTubeOptions.DELAY, value);
            }
        }

        public override QueueType QueueType { get; protected set; } = QueueType.utubettl;

        protected override void ValidateOptionName(string optionName)
        {
            if (!s_utubeTtlOptions.Contains(optionName))
                ThrowValidateOptionNameException(optionName);
        }
    }
}
