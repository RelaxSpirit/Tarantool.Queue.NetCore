using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tarantool.Queues.Model;

namespace Tarantool.Queues.Options
{
    public class LimFiFoTtlTubeOptions : FiFoTtlTubeOptions
    {
        const string TIMEOUT = "timeout";

        private static readonly string[] s_limFifoTtlOptions = s_fifoTtlOptions.Concat(new string[]
        {
            TIMEOUT
        }).ToArray();

        public LimFiFoTtlTubeOptions() : base() { }

        public override QueueType QueueType => QueueType.limfifottl;

        /// <summary>
        /// Секунды ожидания, пока в очереди освободится место
        /// </summary>
        public TimeSpan? Timeout
        {
            get
            {
                return GetTimeSpanValue(TIMEOUT);
            }
            set
            {
                SetTimeSpanValue(TIMEOUT, value);
            }
        }
        protected override void ValidateOptionName(string optionName)
        {
            if (!s_limFifoTtlOptions.Contains(optionName))
                ThrowValidateOptionNameException(optionName);
        }
    }
}
