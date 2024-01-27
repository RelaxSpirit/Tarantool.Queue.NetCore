using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarantool.Queues.Model
{
    public enum QueueTaskState : ushort
    {
        READY = 'r',
        TAKEN = 't',
        DONE = '-',
        BURIED = '!',
        DELAYED = '~',
    }
}
