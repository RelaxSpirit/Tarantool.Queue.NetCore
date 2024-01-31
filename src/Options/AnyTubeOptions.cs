using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tarantool.Queues.Model;

namespace Tarantool.Queues.Options
{
    public class AnyTubeOptions : TubeOptions
    {
        public override QueueType QueueType { get; protected set; } = QueueType.customtube;

        public AnyTubeOptions()
        {

        }

        protected override void ValidateOptionName(string optionName)
        {
        }

        internal static TubeOptions Empty => new AnyTubeOptions();
    }
}
