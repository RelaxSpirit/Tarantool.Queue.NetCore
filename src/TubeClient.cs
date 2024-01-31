using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public abstract class TubeClient<TQueueTubeOption> : ITubeClient
        where TQueueTubeOption : TubeOptions
    {
        protected readonly ITube _queueTube;
        protected readonly TQueueTubeOption _defaultOptions;

        protected TubeClient(ITube queueTube)
        {
            _queueTube = queueTube;
            if (_queueTube.TubeType != QueueType.customtube)
                _defaultOptions = (TQueueTubeOption)TubeOptions.GetDefaultTubeOptions(queueTube.TubeType);
            else
                _defaultOptions = (TQueueTubeOption)AnyTubeOptions.Empty;
        }

        public async Task<QueueTubeStatistic> GetStatistics()
            => await _queueTube.GetStatistics();

        protected void CheckTubeType(QueueType queueType)
        {
            if (_queueTube.TubeType != queueType)
                throw new NotSupportedException($"Received tube name '{_queueTube.Name}' and type '{_queueTube.TubeType}' is not type '{queueType}'");
        }
    }
}
