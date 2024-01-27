using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tarantool.Queues.Model;

namespace Tarantool.Queues.Options
{
    public abstract class AnyTubeOptions : TubeOptions
    {
        public override QueueType QueueType { get; protected set; } = QueueType.customtube;

        public AnyTubeOptions()
        {

        }

    }
}
